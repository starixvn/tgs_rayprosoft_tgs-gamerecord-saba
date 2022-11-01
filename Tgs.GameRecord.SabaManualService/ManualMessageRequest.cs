namespace Tgs.GameRecord.SabaManualService;

public record ManualMessageRequest(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
