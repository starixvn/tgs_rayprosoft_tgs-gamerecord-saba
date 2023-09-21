namespace Tgs.GameRecord.Saba.Core.JobApplicationServices;

public record JobExecuteCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId);
