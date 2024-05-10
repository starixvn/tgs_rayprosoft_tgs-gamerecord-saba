namespace Tgs.GameRecord.Saba.Core;

public record GameRecordInfoOptions
{
	public string GameProviderCode { get; set; } = default!;
	public string GameExternalId { get; set; } = default!;
	public string GameProviderConfiguration { get; set; } = default!;

	public string HangfirePrefix { get; set; } = "gamerecord-saba-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-saba-locking:";

	public long VersionKey { get; set; } = 0;

	public double LockMinute { get; set; } = 10d;
}
