using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Saba.Common;

public record SabaGameRecordScheduleMessage
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
}
