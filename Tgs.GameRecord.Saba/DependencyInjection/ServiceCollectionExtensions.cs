using Tgs.GameCommand.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameRecord.Saba;
using Tgs.GameRecord.Saba.Core;
using Tgs.GameRecordSchedules.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecord(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameInterfaceSabaClientOptions, IServiceProvider> gameInterfaceSabaClientOptions,
        Action<GameRecordSchedulesClientOptions, IServiceProvider> gameRecordSchedulesClientOptions)
        => services
        .AddOptions<RedisOptions>()
        .Configure(redisDBconfigure)
        .Services
        .AddOptions<GameRecordInfoOptions>()
        .Configure(gameRecordInfoOptions)
        .Services
        .AddSingleton<RedisConnectionManager>()
        .AddGameRecordCore(
            gameCommandClientOptions: gameCommandClientOptions,
            gameInterfaceSabaClientOptions: gameInterfaceSabaClientOptions,
            gameRecordSchedulesClientOptions: gameRecordSchedulesClientOptions,
            redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
