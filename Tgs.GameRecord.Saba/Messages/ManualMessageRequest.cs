namespace Tgs.GameRecord.Saba.Messages;

public record ManualMessageRequest(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
