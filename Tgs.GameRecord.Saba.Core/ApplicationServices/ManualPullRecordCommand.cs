namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

public record ManualPullRecordCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
