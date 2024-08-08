using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tgs.GameRecord.Saba.Repositories.Models;
[Table("game_record_schedules")]
public record GameRecordSchedule
{
	/// <summary>
	/// The identifier
	/// </summary>
	[BsonId]
	public ObjectId Id { get; set; } = default!;
	/// <summary>
	/// The operator identifier
	/// </summary>
	[BsonElement("operator_id")]
	public string OperatorId { get; set; } = default!;
	/// <summary>
	/// The vendor identifier
	/// </summary>
	[BsonElement("vendor_id")]
	public string VendorId { get; set; } = default!;
	/// <summary>
	/// The version key
	/// </summary>
	[BsonElement("version_key")]
	public long VersionKey { get; set; } = default!;
	/// <summary>
	/// The created at
	/// </summary>
	[BsonElement("created_at")]
	public DateTime CreatedAt { get; init; } = default!;
	/// <summary>
	/// The updated at
	/// </summary>
	[BsonElement("updated_at")]
	public DateTime UpdatedAt { get; init; } = default!;
}
