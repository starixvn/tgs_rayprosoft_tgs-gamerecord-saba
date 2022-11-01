namespace Tgs.GameRecord.SabaService.Core.WorkerServices;

public record GameHistoryWorkerQuery(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    long VersionKey);
