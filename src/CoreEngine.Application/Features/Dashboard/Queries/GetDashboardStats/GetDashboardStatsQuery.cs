using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Dashboard.Queries.GetDashboardStats;

public record DashboardStatsDto(
    int UserCount,
    int RoleCount,
    int DepartmentCount,
    int AuditLogCount,
    int FileCount,
    int ReportCount,
    int ActiveTaskCount,
    int EnabledFeatureFlagCount,
    int ActiveApiKeyCount,
    List<RecentActivityDto> RecentActivity
);

public record RecentActivityDto(string Action, string EntityName, string EntityId, DateTime Timestamp);

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var userCount = await _context.Users.CountAsync(ct);
        var roleCount = await _context.Roles.CountAsync(ct);
        var deptCount = await _context.Departments.CountAsync(ct);
        var auditCount = await _context.AuditLogs.CountAsync(ct);
        var fileCount = await _context.FileMetadata.CountAsync(ct);
        var reportCount = await _context.ReportDefinitions.CountAsync(ct);
        var activeTaskCount = await _context.DemoTasks.CountAsync(t => t.CurrentState != "Done" && t.CurrentState != "Cancelled", ct);
        var enabledFlagCount = await _context.FeatureFlags.CountAsync(f => f.IsEnabled, ct);
        var activeKeyCount = await _context.ApiKeys.CountAsync(k => k.IsActive, ct);

        var recentActivity = await _context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(5)
            .Select(a => new RecentActivityDto(a.Action, a.EntityName, a.EntityId, a.Timestamp))
            .ToListAsync(ct);

        return new DashboardStatsDto(
            userCount, roleCount, deptCount, auditCount, fileCount, reportCount,
            activeTaskCount, enabledFlagCount, activeKeyCount, recentActivity
        );
    }
}
