using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.GameRecordSchedules.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

internal class SchedulePullRecordCommandService(
    ILogger<SchedulePullRecordCommandService> logger,
    IOptions<GameRecordInfoOptions> options,
    ISchedulePullLocked schedulePullLocked,
    ISabaGameRecordSchedulesClient sabaGameRecordSchedulesClient,
    IGameInterfaceSabaClient gameInterfaceSabaClient,
    ICommandService<HandleRecordCommand> service) : ICommandService<SchedulePullRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

    public async ValueTask ExecuteAsync(SchedulePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        if (!(await schedulePullLocked.LockingAsync(
            operatorId: command.OperatorId,
            command: command)
            .ConfigureAwait(false)))
        {
            logger.LogWarning("LogOn:{logOn} Message:This operator is locked.({operatorId})", DateTime.Now.ToString("o"), command.OperatorId);
            return;
        }

        try
        {
            var versionKey = await sabaGameRecordSchedulesClient.GetAsync(
                operatorId: command.OperatorId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (versionKey < _gameRecordInfo.VersionKey)
            {
                versionKey = _gameRecordInfo.VersionKey;
            }

            logger.LogInformation("{command.OperatorId}取得VersionKey:{versionKey}", command.OperatorId, versionKey);

            long tempVersionKey;
            do
            {
                var reply = await gameInterfaceSabaClient.GameHistoryAsync(
                    apiUrl: command.ApiUrl,
                    operatorId: command.OperatorId,
                    vendorId: command.VendorId,
                    versionKey: versionKey,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var gameHistories = reply.Items ?? Enumerable.Empty<GameHistory>();

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
                tempVersionKey = reply.LastVersionKey;
            } while (tempVersionKey != versionKey);

            await sabaGameRecordSchedulesClient.MergeAsync(
                operatorId: command.OperatorId,
                versionKey: versionKey,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            await schedulePullLocked.UnlockAsync(operatorId: command.OperatorId).ConfigureAwait(false);
        }
    }
}
