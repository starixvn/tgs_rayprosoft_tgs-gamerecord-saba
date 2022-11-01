using Tgs.GameRecord.SabaService.Core.DispatchServices;

namespace Tgs.GameRecord.SabaService.Core;

public interface IScheduleDispatch
{
    public Task<bool> ExistAsync(string key);

    public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command);

    public Task UnlockAsync(string key);
}
