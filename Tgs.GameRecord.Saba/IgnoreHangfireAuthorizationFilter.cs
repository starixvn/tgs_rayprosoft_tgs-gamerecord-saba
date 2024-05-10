using Hangfire.Dashboard;

namespace Tgs.GameRecord.Saba;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
