using Tgs.GameRecord.Saba.Core.ApplicationServices;

namespace Tgs.GameRecord.Saba.Core;

public interface IManualPullRecordDispatcher
{
	ValueTask PushCommandAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default);
}
