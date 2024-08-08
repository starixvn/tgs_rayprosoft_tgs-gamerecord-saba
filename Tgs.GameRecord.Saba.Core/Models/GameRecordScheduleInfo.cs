namespace Tgs.GameRecord.Saba.Core.Models;
public record GameRecordScheduleInfo(
	string OperatorId,
	string VendorId,
	long VersionKey,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt);
