using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Saba.Common;

public record SabaGameRecordManualMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "http://i1y2tsa.bw6688.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "Pega")]
	public string OperatorId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "lmaz5farkr")]
	public string VendorId { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-08-24T18:15:00+08:00")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-08-24T19:30:00+08:00")]
	public DateTimeOffset EndTime { get; init; }
}
