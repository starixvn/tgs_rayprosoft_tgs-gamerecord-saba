namespace Tgs.GameRecord.SabaService.Core;

public record NatsSettingOptions
{
    public string Group { get; set; } = default!;
    public string Subject { get; set; } = default!;
}
