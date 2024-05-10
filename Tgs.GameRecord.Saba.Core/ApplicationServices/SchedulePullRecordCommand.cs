namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

public record SchedulePullRecordCommand(
	Uri ApiUrl,
	string OperatorId,
	string VendorId);
