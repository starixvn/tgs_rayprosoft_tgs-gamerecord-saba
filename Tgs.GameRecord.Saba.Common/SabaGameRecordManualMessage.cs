using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Saba.Common;

public record SabaGameRecordManualMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string OperatorId { get; init; } = default!;
	[Required]
	public string VendorId { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}
