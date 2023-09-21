using Tgs.GameRecord.Saba.Core.JobApplicationServices;

namespace Tgs.GameRecord.Saba.Core;

public interface IHangfireUtensil
{
    public Task JobCheckAsync();

    public Task JobExecuteAsync(JobExecuteCommand source);

    public Task JobInitAsync();
}
