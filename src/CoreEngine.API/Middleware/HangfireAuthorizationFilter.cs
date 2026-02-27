using Hangfire.Dashboard;

namespace CoreEngine.API.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Check if user is authenticated
        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            return false;

        // Check if user has BackgroundJob.Read permission
        var hasPermission = httpContext.User.Claims
            .Any(c => c.Type == "permission" && c.Value == "BackgroundJob.Read");

        return hasPermission;
    }
}
