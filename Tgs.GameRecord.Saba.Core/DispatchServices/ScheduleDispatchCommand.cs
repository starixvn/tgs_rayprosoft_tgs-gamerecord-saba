namespace Tgs.GameRecord.Saba.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId);
