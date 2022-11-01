using Microsoft.Extensions.Options;
using Tgs.GameRecord.SabaService.Core.WorkerServices;
using Tgs.GameRecordSchedules.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SabaService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IGetService<GameHistoryWorkerQuery, long> _service;
    private readonly IScheduleDispatch _scheduleDispatch;
    private readonly ISabaGameRecordSchedulesClient _client;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        IGetService<GameHistoryWorkerQuery, long> service,
        IScheduleDispatch scheduleDispatch,
        ISabaGameRecordSchedulesClient client)
    {
        _gameRecordInfo = options.Value;
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        _client = client;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"gp:gamerecord:{_gameRecordInfo.GameProviderCode}:{command.OperatorId}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                var versionKey = await _client
                                    .GetAsync(
                                        operatorId: command.OperatorId,
                                        cancellationToken: cancellationToken)
                                    .ConfigureAwait(false);

                if (versionKey < _gameRecordInfo.VersionKey)
                {
                    versionKey = _gameRecordInfo.VersionKey;
                }
                try
                {
                    while (true)
                    {
                        var query = new GameHistoryWorkerQuery(
                             ApiUrl: command.ApiUrl,
                             OperatorId: command.OperatorId,
                             VendorId: command.VendorId,
                             VersionKey: versionKey);

                        var tempVersionKey = await _service
                                            .GetAsync(
                                                query: query,
                                                cancellationToken: cancellationToken)
                                            .ConfigureAwait(false);

                        if (tempVersionKey == versionKey)
                        {
                            break;
                        }
                        versionKey = tempVersionKey;

                        await _client
                            .MergeAsync(
                                operatorId: command.OperatorId,
                                versionKey: versionKey,
                                cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                finally
                {
                    await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
