using Tgs.GameRecord.Saba.Core.ApplicationServices;

namespace Tgs.GameRecord.Saba.Core;

public interface ISchedulePullLocked
{
	public Task<bool> LockingAsync(string operatorId, SchedulePullRecordCommand command);

	public Task UnlockAsync(string operatorId);
}
