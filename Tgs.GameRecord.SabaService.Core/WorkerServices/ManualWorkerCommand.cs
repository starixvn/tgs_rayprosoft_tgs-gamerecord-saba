namespace Tgs.GameRecord.SabaService.Core.WorkerServices;

public record ManualWorkerCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
