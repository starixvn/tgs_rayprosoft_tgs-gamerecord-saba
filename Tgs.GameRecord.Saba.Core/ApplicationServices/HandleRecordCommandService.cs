using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfoOptions = gameRecordInfoOptions.Value;
	private readonly PolicySettingsOptions _policySettingsOptions = policySettingsOptions.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var betState = Handle.Common.Protos.BetState.Running;
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
		var settle = 0.0m;
		var turnover = 0.0m;
		var winLoss = 0.0m;
		var cashoutState = Handle.Common.Protos.CashoutState.None;
		if (command.Record.IsClosed)
		{
			externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
			settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
			turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
			winLoss = command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0);
			cashoutState = command.Record.IsCashout ? Handle.Common.Protos.CashoutState.Cashout : Handle.Common.Protos.CashoutState.None;
			betState = command.Record.Status switch
			{
				GameHistoryStatus.Won => Handle.Common.Protos.BetState.Win,
				GameHistoryStatus.Lose => Handle.Common.Protos.BetState.Loss,
				GameHistoryStatus.Draw => Handle.Common.Protos.BetState.Draw,
				GameHistoryStatus.HalfWon => Handle.Common.Protos.BetState.HalfWin,
				GameHistoryStatus.HalfLose => Handle.Common.Protos.BetState.HalfLoss,
				GameHistoryStatus.Void => Handle.Common.Protos.BetState.Void,
				GameHistoryStatus.Refund => Handle.Common.Protos.BetState.Refund,
				GameHistoryStatus.Reject => Handle.Common.Protos.BetState.Cancel,
				_ => (winLoss > 0) ? Handle.Common.Protos.BetState.Win : Handle.Common.Protos.BetState.Loss
			};
		}

		var source = new Handle.Common.ViewModels.GameRecordHandleViewModel
		{
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			PlayerCode = command.Record.PlayerCode,
			ExternalRoundId = command.Record.ExternalRoundId,
			ExternalBetId = command.Record.ExternalRoundId,
			ExternalGameId = _gameRecordInfoOptions.GameExternalId,
			ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = cashoutState,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.Record.AdditionalData
		};

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "{logWarning}",
					new
					{
						LogOn = DateTime.Now.ToString("u"),
						RetryTime = retryTime,
						RetryCount = retryCount,
						Data = new
						{
							source.GameProviderCode,
							source.PlayerCode,
							source.ExternalRoundId,
							source.ExternalBetId,
							source.ExternalGameId,
							ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
							ExternalClosedOn = source.ExternalClosedOn.ToString("u"),
							source.BetState
						},
						exception.Message
					}));

		await retryPolicy.ExecuteAsync(() => client.HandleAsync(
			source: source,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}
}
