using Tgs.GameInterface.SabaService.Client;
using Tgs.GameRecord.Saba.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Saba.Core.WorkerServices;

internal class ManualWorkerCommandService : ICommandService<ManualWorkerCommand>
{
    private readonly IGameInterfaceSabaClient _client;
    private readonly ICommandService<PullRecordCommand> _service;

    public ManualWorkerCommandService(
        IGameInterfaceSabaClient client,
        ICommandService<PullRecordCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(ManualWorkerCommand command, CancellationToken cancellationToken = default)
    {
        var gameHistories = await _client
                .GameHistoryUnsettledAsync(
                    apiUrl: command.ApiUrl,
                    operatorId: command.OperatorId,
                    vendorId: command.VendorId,
                    startTime: command.StartTime.ToUniversalTime(),
                    endTime: command.EndTime.ToUniversalTime(),
                    cancellationToken: cancellationToken)
                .ToArrayAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (gameHistories.Any())
        {
            var gamePullRecordCommand = new PullRecordCommand(
                GameHistories: gameHistories);

            await _service
                .ExecuteAsync(
                    command: gamePullRecordCommand,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        gameHistories = await _client
                .GameHistorySettledAsync(
                    apiUrl: command.ApiUrl,
                    operatorId: command.OperatorId,
                    vendorId: command.VendorId,
                    startTime: command.StartTime.ToUniversalTime(),
                    endTime: command.EndTime.ToUniversalTime(),
                    cancellationToken: cancellationToken)
                .ToArrayAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (gameHistories.Any())
        {
            var gamePullRecordCommand = new PullRecordCommand(
                GameHistories: gameHistories);

            await _service
                .ExecuteAsync(
                    command: gamePullRecordCommand,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
