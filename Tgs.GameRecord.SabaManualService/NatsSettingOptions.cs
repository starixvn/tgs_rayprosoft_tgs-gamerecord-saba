namespace Tgs.GameRecord.SabaManualService;

public record NatsSettingOptions
{
    public string Queue { get; set; } = default!;

    public string Subject { get; set; } = default!;
}
