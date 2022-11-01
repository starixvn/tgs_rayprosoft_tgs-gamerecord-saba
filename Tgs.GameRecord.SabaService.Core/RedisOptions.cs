namespace Tgs.GameRecord.SabaService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
