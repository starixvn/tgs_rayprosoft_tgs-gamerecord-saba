using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tgs.GameRecord.Saba.Core.ApplicationServices;

namespace Tgs.GameRecord.Saba.Core;

internal class SchedulePullLocked(
    IOptions<GameRecordInfoOptions> options,
    IDatabase database) : ISchedulePullLocked
{
    private readonly GameRecordInfoOptions _gameRecordInfoOptions = options.Value;

    public Task<bool> LockingAsync(string operatorId, SchedulePullRecordCommand command)
        => database.StringSetAsync(
            key: GetKey(operatorId),
            value: JsonSerializer.Serialize(command),
            expiry: TimeSpan.FromMinutes(_gameRecordInfoOptions.LockMinute),
            when: When.NotExists);

    public async Task UnlockAsync(string operatorId)
        => _ = await database.KeyDeleteAsync(
            key: GetKey(operatorId),
            flags: CommandFlags.FireAndForget)
        .ConfigureAwait(false);

    private static string GetKey(string operatorId)
        => string.Concat("gamerecord-saba-operatorid:", operatorId);
}
