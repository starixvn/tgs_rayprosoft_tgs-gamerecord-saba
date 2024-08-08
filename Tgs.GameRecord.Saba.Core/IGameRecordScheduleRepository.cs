using Tgs.GameRecord.Saba.Core.Models;

namespace Tgs.GameRecord.Saba.Core;

public interface IGameRecordScheduleRepository
{
	ValueTask<GameRecordScheduleInfo?> GetAsync(string operatorId, string vendorId, CancellationToken cancellationToken = default);

	ValueTask<GameRecordScheduleInfo> AddAsync(string operatorId, string vendorId, CancellationToken cancellationToken = default);

	ValueTask InitializationAsync(CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(string operatorId, string vendorId, long versionKey, CancellationToken cancellationToken = default);
}
