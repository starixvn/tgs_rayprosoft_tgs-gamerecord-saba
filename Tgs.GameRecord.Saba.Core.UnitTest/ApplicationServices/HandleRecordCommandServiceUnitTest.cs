using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Saba.Core.ApplicationServices;

namespace Tgs.GameRecord.Saba.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Cashout()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "SABATF",
            GameExternalId = "sports_book",
            VersionKey = 0,
            LockMinute = 10
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

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

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command: command);

        await fakeClient
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Record.PlayerCode),
            wagerState: Arg.Is(WagerStates.Refund),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.Cashout),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalWagerId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Lose()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "SABATF",
            GameExternalId = "sports_book",
            VersionKey = 0,
            LockMinute = 10
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

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

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command: command);

        await fakeClient
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Record.PlayerCode),
            wagerState: Arg.Is(WagerStates.Lose),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalWagerId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Running()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "SABATF",
            GameExternalId = "sports_book",
            VersionKey = 0,
            LockMinute = 10
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(new GameHistory
        {
            PlayerCode = "4ucmqplxb029",
            ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            IsCashout = false,
            IsClosed = false,
            Status = GameHistoryStatus.Running,
            ExternalCreatedOn = DateTimeOffset.UtcNow.AddHours(-1).ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.UtcNow.ToTimestamp(),
            Bet = 10m,
            ValidBet = 10m,
            PlayerWinLoss = 0m,
            Turnover = 0m,
            Settle = 0m,
            AdditionalData = JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" })
        });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command: command);

        await fakeClient
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Record.PlayerCode),
            wagerState: Arg.Is(WagerStates.Running),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalWagerId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Won()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "SABATF",
            GameExternalId = "sports_book",
            VersionKey = 0,
            LockMinute = 10
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

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

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service.ExecuteAsync(command: command);

        await fakeClient
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(command.Record.PlayerCode),
            wagerState: Arg.Is(WagerStates.Win),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalWagerId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(gameRecordInfo.Value.GameExternalId),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            winLoss: Arg.Is(command.Record.PlayerWinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
