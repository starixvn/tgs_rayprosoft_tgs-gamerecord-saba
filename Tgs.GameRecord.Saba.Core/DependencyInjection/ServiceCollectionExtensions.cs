using System.Reflection;
using StackExchange.Redis;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameRecord.Saba.Core;
using Tgs.GameRecordSchedules.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordCore(
		this IServiceCollection services,
		Action<GameInterfaceSabaClientOptions, IServiceProvider> gameInterfaceSabaClientOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameRecordSchedulesClientOptions, IServiceProvider> gameRecordSchedulesClientOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameRecordSchedulesClient(gameRecordSchedulesClientOptions)
		.AddGameInterfaceSabaServiceClient(gameInterfaceSabaClientOptions)
		.AddTransient<IHangfireUtensil, HangfireUtensil>()
		.AddTransient<IHangfireLock>(sp => ActivatorUtilities.CreateInstance<HangfireLock>(sp, redisDatabaseResolver(sp)))
		.AddTransient<ISchedulePullLocked>(sp => ActivatorUtilities.CreateInstance<SchedulePullLocked>(sp, redisDatabaseResolver(sp)));
}
