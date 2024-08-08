using Tgs.GameRecord.Saba.Core.ApplicationServices;

namespace Tgs.GameRecord.Saba.Core;

public interface ISchedulePullRecordDispatcher
{
	ValueTask PushCommandAsync(SchedulePullRecordCommand command, CancellationToken cancellationToken = default);
}
