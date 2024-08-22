using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Saba.Core.ApplicationServices;

namespace Tgs.GameRecord.Saba.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Cashout()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SABATF",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-saba-hangfire:",
			HangfireLocking = "gamerecord-saba-locking:",
			VersionKey = 0,
			LockMinute = 10
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(new GameHistory
		{
			PlayerCode = "4ucmqplxb029",
			ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			IsCashout = true,
			IsClosed = true,
			Status = GameHistoryStatus.Refund,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddHours(-1).ToTimestamp(),
			ExternalClosedOn = DateTimeOffset.UtcNow.ToTimestamp(),
			Bet = 10m,
			ValidBet = 0m,
			PlayerWinLoss = 0m,
			Turnover = 0m,
			Settle = 0m,
			AdditionalData = JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" })
		});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Refund,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.Cashout,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10m,
				ValidBet = 0.0M,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.0M,
				WinLoss = 0.0M,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SABATF",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-saba-hangfire:",
			HangfireLocking = "gamerecord-saba-locking:",
			VersionKey = 0,
			LockMinute = 10
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(new GameHistory
		{
			PlayerCode = "4ucmqplxb029",
			ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			IsCashout = false,
			IsClosed = true,
			Status = GameHistoryStatus.Lose,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddHours(-1).ToTimestamp(),
			ExternalClosedOn = DateTimeOffset.UtcNow.ToTimestamp(),
			Bet = 10m,
			ValidBet = 10m,
			PlayerWinLoss = -10m,
			Turnover = 10m,
			Settle = 0m,
			AdditionalData = JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" })
		});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10m,
				ValidBet = 10m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 10m,
				WinLoss = -10m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Running()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SABATF",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-saba-hangfire:",
			HangfireLocking = "gamerecord-saba-locking:",
			VersionKey = 0,
			LockMinute = 10
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(new GameHistory
		{
			PlayerCode = "4ucmqplxb029",
			ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			IsCashout = false,
			IsClosed = false,
			Status = GameHistoryStatus.Running,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddHours(-1).ToTimestamp(),
			ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			Bet = 10m,
			ValidBet = 10m,
			PlayerWinLoss = 0m,
			Turnover = 0m,
			Settle = 0m,
			AdditionalData = JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" })
		});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = DateTimeOffset.UnixEpoch,
				ExternalPayoffOn = DateTimeOffset.UnixEpoch,
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10m,
				ValidBet = 10m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.0M,
				WinLoss = 0.0M,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SABATF",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-saba-hangfire:",
			HangfireLocking = "gamerecord-saba-locking:",
			VersionKey = 0,
			LockMinute = 10
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(new GameHistory
		{
			PlayerCode = "4ucmqplxb029",
			ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			IsCashout = false,
			IsClosed = true,
			Status = GameHistoryStatus.Won,
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddHours(-1).ToTimestamp(),
			ExternalClosedOn = DateTimeOffset.UtcNow.ToTimestamp(),
			Bet = 10m,
			ValidBet = 10m,
			PlayerWinLoss = 10m,
			Turnover = 10m,
			Settle = 20m,
			AdditionalData = JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" })
		});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = fakeGameRecordInfoOptions.Value.GameExternalId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10m,
				ValidBet = 10m,
				Settle = 20m,
				Commission = 0.0M,
				Turnover = 10m,
				WinLoss = 10m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
