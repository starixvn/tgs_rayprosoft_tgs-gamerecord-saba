namespace Tgs.GameRecord.SabaManualService.ViewModels;

public record GameRecordQueryViewModel(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
