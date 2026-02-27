using Hangfire.Dashboard;

namespace CoreEngine.API.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _environment;

    public HangfireAuthorizationFilter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow unrestricted access in Development mode
        if (_environment.IsDevelopment())
            return true;

        // In production, require authentication and permission
        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            return false;

        // Check if user has BackgroundJob.Read permission
        var hasPermission = httpContext.User.Claims
            .Any(c => c.Type == "permission" && c.Value == "BackgroundJob.Read");

        return hasPermission;
    }
}
