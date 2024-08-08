namespace Tgs.GameRecord.Saba;

public record MongoDBOptions
{
	public string ConnectionString { get; set; } = "mongodb://localhost";
}
