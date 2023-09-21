namespace Tgs.GameRecord.Saba.Messages;

public record ScheduleMessageModel(
    Uri ApiUrl,
    string OperatorId,
    string VendorId);
