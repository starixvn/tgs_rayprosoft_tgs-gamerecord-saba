using Tgs.GameRecord.Saba.Core;

namespace Tgs.GameRecord.Saba;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
