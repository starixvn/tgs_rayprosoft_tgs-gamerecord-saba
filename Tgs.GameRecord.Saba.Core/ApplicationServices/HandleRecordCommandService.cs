using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

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

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.Record.PlayerCode,
			ExternalRoundId = command.Record.ExternalRoundId,
			ExternalBetId = command.Record.ExternalRoundId,
			ExternalGameId = _gameRecordInfo.GameExternalId,
			ExternalCreatedOn = command.Record.ExternalCreatedOn,
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
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

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
			});
	}
}
