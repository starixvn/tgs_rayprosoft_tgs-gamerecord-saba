using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Saba.Common;

public record SabaGameRecordScheduleMessage
{
    [Required]
    [DataType(DataType.Url)]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string OperatorId { get; init; } = default!;
    [Required]
    public string VendorId { get; init; } = default!;
}
