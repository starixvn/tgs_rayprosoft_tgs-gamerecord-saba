using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SabaService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly ICommandService<ScheduleHandleRecordCommand> _service;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        ICommandService<ScheduleHandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var history in command.GameHistories)
        {
            try
            {
                await _service
                .ExecuteAsync(
                     command: new ScheduleHandleRecordCommand(history),
                     cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "遊戲紀錄處理發生例外.({history})", history);
            }
        }
    }
}
