using CoreEngine.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreEngine.Infrastructure.BackgroundJobs;

public class CleanupAuditLogsJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CleanupAuditLogsJob> _logger;

    public CleanupAuditLogsJob(IApplicationDbContext context, ILogger<CleanupAuditLogsJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("CleanupAuditLogsJob started");
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-90);
            var oldLogs = await _context.AuditLogs
                .Where(a => a.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CleanupAuditLogsJob completed. Deleted {Count} audit logs", oldLogs.Count);
            }
            else
            {
                _logger.LogInformation("CleanupAuditLogsJob completed. No old audit logs to delete");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CleanupAuditLogsJob failed");
        }
    }
}
