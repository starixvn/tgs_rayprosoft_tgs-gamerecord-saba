using Microsoft.Extensions.Hosting;
using Tgs.GameRecord.Saba.Core;

namespace Tgs.GameRecord.Saba.Repositories;

public sealed class GameRecordScheduleInitStartup(IGameRecordScheduleRepository gameRecordScheduleRepository) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await gameRecordScheduleRepository.InitializationAsync(
			cancellationToken)
			.ConfigureAwait(false);
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
