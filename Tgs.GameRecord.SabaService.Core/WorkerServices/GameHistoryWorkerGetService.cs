using Tgs.GameInterface.SabaService.Client;
using Tgs.GameRecord.SabaService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SabaService.Core.WorkerServices;

internal class GameHistoryWorkerGetService : IGetService<GameHistoryWorkerQuery, long>
{
    private readonly IGameInterfaceSabaClient _client;
    private readonly ICommandService<PullRecordCommand> _service;

    public GameHistoryWorkerGetService(
        IGameInterfaceSabaClient client,
        ICommandService<PullRecordCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<long> GetAsync(GameHistoryWorkerQuery query, CancellationToken cancellationToken = default)
    {
        var reply = await _client
                .GameHistoryAsync(
                    apiUrl: query.ApiUrl,
                    operatorId: query.OperatorId,
                    vendorId: query.VendorId,
                    versionKey: query.VersionKey,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (reply.Items.Any())
        {
            var command = new PullRecordCommand(
                GameHistories: reply.Items);

            await _service
                .ExecuteAsync(
                    command: command,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        return reply.LastVersionKey;
    }
}
