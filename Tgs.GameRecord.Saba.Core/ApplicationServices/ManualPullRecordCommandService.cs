using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

internal class ManualPullRecordCommandService(
    ILogger<ManualPullRecordCommandService> logger,
    IGameInterfaceSabaClient client,
    ICommandService<HandleRecordCommand> service) : ICommandService<ManualPullRecordCommand>
{
    public async ValueTask ExecuteAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default)
    {
        IEnumerable<GameHistory> gameHistories;

        gameHistories = await client.GameHistoryUnsettledAsync(
           apiUrl: command.ApiUrl,
           operatorId: command.OperatorId,
           vendorId: command.VendorId,
           startTime: command.StartTime.UtcDateTime.ToUniversalTime(),
           endTime: command.EndTime.UtcDateTime.ToUniversalTime(),
           cancellationToken: cancellationToken)
           .ToArrayAsync(
           cancellationToken: cancellationToken)
           .ConfigureAwait(false)
           ?? Enumerable.Empty<GameHistory>();

        if (gameHistories.Any())
        {
            foreach (var gameHistory in gameHistories)
            {
                try
                {
                    await service.ExecuteAsync(
                        command: new HandleRecordCommand(gameHistory),
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "遊戲紀錄處理發生例外.({history})", gameHistory);
                }
            }
        }

        gameHistories = await client.GameHistorySettledAsync(
            apiUrl: command.ApiUrl,
            operatorId: command.OperatorId,
            vendorId: command.VendorId,
            startTime: command.StartTime.UtcDateTime.ToUniversalTime(),
            endTime: command.EndTime.UtcDateTime.ToUniversalTime(),
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistory>();

        if (gameHistories.Any())
        {
            foreach (var gameHistory in gameHistories)
            {
                try
                {
                    await service.ExecuteAsync(
                        command: new HandleRecordCommand(gameHistory),
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "遊戲紀錄處理發生例外.({history})", gameHistory);
                }
            }
        }
    }
}
