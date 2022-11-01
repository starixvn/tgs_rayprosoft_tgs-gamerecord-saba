namespace Tgs.GameRecord.SabaService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId);
