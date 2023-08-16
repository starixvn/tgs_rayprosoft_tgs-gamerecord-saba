using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SabaService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var status = (command.Record.IsClosed) ? GameRecordStatus.Closed : GameRecordStatus.Open;
        var clientStatus = GameRecordClientStatus.Open;
        var externalClosedOn = (DateTimeOffset?)null;
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var playerWinLoss = 0.0m;
        if (command.Record.IsClosed)
        {
            externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
            settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
            commission = 0.0m;
            turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
            playerWinLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
            clientStatus =
               (command.Record.IsClosed && command.Record.IsCashout)
               ? (playerWinLoss > 0) ? GameRecordClientStatus.Win : GameRecordClientStatus.Loss
               : command.Record.Status switch
               {
                   GameHistoryStatus.Won => GameRecordClientStatus.Win,
                   GameHistoryStatus.Lose => GameRecordClientStatus.Loss,
                   GameHistoryStatus.Draw => GameRecordClientStatus.Draw,
                   GameHistoryStatus.HalfWon => GameRecordClientStatus.Win,
                   GameHistoryStatus.HalfLose => GameRecordClientStatus.Loss,
                   GameHistoryStatus.Void => GameRecordClientStatus.Void,
                   GameHistoryStatus.Refund => GameRecordClientStatus.Void,
                   GameHistoryStatus.Reject => GameRecordClientStatus.Void,
                   _ => (command.Record.PlayerWinLoss > 0) ? GameRecordClientStatus.Win : GameRecordClientStatus.Loss
               };
        }

        await _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            externalRoundId: command.Record.ExternalRoundId,
            externalGameId: _gameRecordInfo.GameExternalId,
            playerCode: command.Record.PlayerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: command.Record.ExternalCreatedOn.ToDateTimeOffset(),
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: playerWinLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
