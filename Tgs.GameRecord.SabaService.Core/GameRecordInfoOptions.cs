namespace Tgs.GameRecord.SabaService.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public string GameExternalId { get; set; } = default!;

    public long VersionKey { get; set; } = 0;

    public double LockMinute { get; set; } = 10d;
}
