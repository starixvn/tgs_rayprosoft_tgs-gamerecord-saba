using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

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

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = WagerStates.Running;
        var externalClosedOn = DateTimeOffset.UnixEpoch;
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var playerWinLoss = 0.0m;
        var cashoutState = CashoutStates.None;
        if (command.Record.IsClosed)
        {
            externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
            settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
            commission = 0.0m;
            turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
            playerWinLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
            cashoutState = (command.Record.IsCashout) ? CashoutStates.Cashout : CashoutStates.None;
            wagerState = command.Record.Status switch
            {
                GameHistoryStatus.Won => WagerStates.Win,
                GameHistoryStatus.Lose => WagerStates.Lose,
                GameHistoryStatus.Draw => WagerStates.Draw,
                GameHistoryStatus.HalfWon => WagerStates.HalfWin,
                GameHistoryStatus.HalfLose => WagerStates.HalfLose,
                GameHistoryStatus.Void => WagerStates.Void,
                GameHistoryStatus.Refund => WagerStates.Refund,
                GameHistoryStatus.Reject => WagerStates.Reject,
                _ => (command.Record.PlayerWinLoss > 0) ? WagerStates.Win : WagerStates.Lose
            };
        }

        return _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Record.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: cashoutState,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.ExternalRoundId,
            externalWagerId: command.Record.ExternalRoundId,
            externalGameId: _gameRecordInfo.GameExternalId,
            externalCreatedOn: command.Record.ExternalCreatedOn.ToDateTimeOffset(),
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: playerWinLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken);
    }
}
