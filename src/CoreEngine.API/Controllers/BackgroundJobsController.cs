using CoreEngine.API.Filters;
using CoreEngine.Infrastructure.BackgroundJobs;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

[Authorize]
public class BackgroundJobsController : BaseApiController
{
    [HttpGet("stats")]
    [RequirePermission("BackgroundJob.Read")]
    public ActionResult<object> GetStats()
    {
        var monitoring = JobStorage.Current.GetMonitoringApi();
        var stats = monitoring.GetStatistics();

        return Ok(new
        {
            succeededJobs = stats.Succeeded,
            failedJobs = stats.Failed,
            processingJobs = stats.Processing,
            scheduledJobs = stats.Scheduled,
            enqueuedJobs = stats.Enqueued,
            serversCount = stats.Servers,
            recurringJobsCount = stats.Recurring,
            deletedJobs = stats.Deleted
        });
    }

    [HttpPost("trigger/process-email-queue")]
    [RequirePermission("BackgroundJob.Manage")]
    public ActionResult TriggerProcessEmailQueue([FromServices] ProcessEmailQueueJob job)
    {
        BackgroundJob.Enqueue(() => job.Execute());
        return Ok(new { message = "Process email queue job triggered" });
    }

    [HttpPost("trigger/cleanup-audit-logs")]
    [RequirePermission("BackgroundJob.Manage")]
    public ActionResult TriggerCleanupAuditLogs([FromServices] CleanupAuditLogsJob job)
    {
        BackgroundJob.Enqueue(() => job.Execute());
        return Ok(new { message = "Cleanup audit logs job triggered" });
    }
}
