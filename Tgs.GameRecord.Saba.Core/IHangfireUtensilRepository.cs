using Tgs.GameRecord.Saba.Core.JobApplicationServices;

namespace Tgs.GameRecord.Saba.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
