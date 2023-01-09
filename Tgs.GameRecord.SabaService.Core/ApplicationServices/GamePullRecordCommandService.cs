using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SabaService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly MongoSettingOptions _mongoSetting;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IOptions<MongoSettingOptions> mongoSettingOptions,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _mongoSetting = mongoSettingOptions.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId,
                _gameRecordInfo.GameExternalId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        foreach (var record in command.GameHistories)
        {
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    record.PlayerCode,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            if (playerModel == null)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                        playerModel.BrandId,
                        gameProviderId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            GameRoundStatus status = (record.IsClosed) ? GameRoundStatus.Closed : GameRoundStatus.Open;
            GameRoundClientStatus clientStatus = (!record.IsClosed)
                ? GameRoundClientStatus.Open
                : (record.IsClosed && record.IsCashout)
                ? (record.PlayerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss
                :
                record.Status switch
                {
                    GameHistoryStatus.Won => GameRoundClientStatus.Win,
                    GameHistoryStatus.Lose => GameRoundClientStatus.Loss,
                    GameHistoryStatus.Draw => GameRoundClientStatus.Draw,
                    GameHistoryStatus.HalfWon => GameRoundClientStatus.Win,
                    GameHistoryStatus.HalfLose => GameRoundClientStatus.Loss,
                    GameHistoryStatus.Void => GameRoundClientStatus.Void,
                    GameHistoryStatus.Refund => GameRoundClientStatus.Void,
                    GameHistoryStatus.Reject => GameRoundClientStatus.Void,
                    _ => (record.PlayerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss
                };

            DateTimeOffset? externalClosedOn = (record.IsClosed) ? record.ExternalClosedOn.ToDateTimeOffset() : null;

            var message = new GameRecordMessage(
                ExternalRoundId: record.ExternalRoundId,
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                GameId: gameId,
                ExternalGameId: _gameRecordInfo.GameExternalId,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: status,
                ClientStatus: clientStatus,
                ExternalCreatedOn: record.ExternalCreatedOn.ToDateTimeOffset(),
                ExternalClosedOn: externalClosedOn,
                BetAmount: record.Bet.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.ValidBet.ToDecimal().GetValueOrDefault(0),
                SettleAmount: record.Settle,
                Commission: 0,
                Turnover: record.Turnover,
                PlayerWinLoss: record.PlayerWinLoss,
                AdditionalData: record.AdditionalData,
                Success: success);

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{message}",
                DateTime.Now.ToString("o"),
                _mongoSetting.GlobPattern,
                message);

            await _messageSender
                .PublishAsync(
                    subject: _mongoSetting.GlobPattern,
                    data: message.ToBson(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
