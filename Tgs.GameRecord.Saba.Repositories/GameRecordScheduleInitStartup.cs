using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Saba.Core;

namespace Tgs.GameRecord.Saba.Repositories;

public sealed class GameRecordScheduleInitStartup(IGameRecordScheduleRepository gameRecordScheduleRepository, IOptions<AppSettings> appSettings) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		if (appSettings.Value.EnableSchedule)
		{
			await gameRecordScheduleRepository.InitializationAsync(
				cancellationToken)
				.ConfigureAwait(false);
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
