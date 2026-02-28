# ⏱️ Background Jobs Module (Hangfire)

Scheduled, recurring, and fire-and-forget background job processing with Hangfire.

---

## 📋 Overview

The Background Jobs module uses Hangfire for reliable background job processing. It handles email sending, report generation, data cleanup, and any long-running tasks that shouldn't block HTTP requests.

---

## ✨ Key Features

- ✅ **Fire-and-forget jobs** - Execute once and forget
- ✅ **Delayed jobs** - Execute after a delay
- ✅ **Recurring jobs** - Cron-based scheduling
- ✅ **Job continuations** - Chain jobs together
- ✅ **Automatic retries** - Configurable retry attempts
- ✅ **Dashboard UI** - Monitor jobs via web interface
- ✅ **SQL Server storage** - Persistent job queue
- ✅ **Multi-tenant aware** - Jobs respect tenant context
- ✅ **Performance optimized** - Parallel job processing

---

## 🗄️ Hangfire Tables

Hangfire automatically creates these tables in CoreEngineDb:

- `Hangfire.Job` - Job definitions
- `Hangfire.State` - Job state transitions
- `Hangfire.JobParameter` - Job arguments
- `Hangfire.JobQueue` - Job queue
- `Hangfire.Server` - Hangfire server instances
- `Hangfire.Set` - Scheduled jobs
- `Hangfire.Hash` - Job metadata
- `Hangfire.Counter` - Job statistics
- `Hangfire.AggregatedCounter` - Aggregated statistics

---

## ⚙️ Configuration

### Backend Configuration

**File:** `src/CoreEngine.API/Program.cs`

```csharp
// Add Hangfire services
builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                SchemaName = "Hangfire"
            });
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
    options.ServerName = $"CoreEngine-{Environment.MachineName}";
    options.Queues = new[] { "critical", "default", "low" };
});

// Map Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "CoreEngine Background Jobs"
});
```

### Authorization Filter

**File:** `src/CoreEngine.API/Filters/HangfireAuthorizationFilter.cs`

```csharp
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow only authenticated users with BackgroundJob.View permission
        var user = httpContext.User;

        return user.Identity?.IsAuthenticated == true &&
               user.HasClaim("permission", "BackgroundJob.View");
    }
}
```

---

## 💼 Job Types

### 1. Fire-and-Forget Jobs

Execute once immediately in the background.

```csharp
public class SendEmailJob
{
    private readonly IEmailService _emailService;

    public async Task Execute(Guid emailQueueId)
    {
        var email = await _emailService.GetEmailByIdAsync(emailQueueId);
        await _emailService.SendAsync(email);
    }
}

// Enqueue the job
BackgroundJob.Enqueue<SendEmailJob>(job => job.Execute(emailId));
```

### 2. Delayed Jobs

Execute once after a specified delay.

```csharp
// Send reminder email in 1 hour
BackgroundJob.Schedule<SendReminderJob>(
    job => job.Execute(userId, taskId),
    TimeSpan.FromHours(1)
);
```

### 3. Recurring Jobs

Execute on a Cron schedule.

```csharp
public class CleanupOldAuditLogsJob
{
    public async Task Execute()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-365);

        await _context.AuditLogs
            .Where(a => a.CreatedAt < cutoffDate)
            .ExecuteDeleteAsync();
    }
}

// Register recurring job
RecurringJob.AddOrUpdate<CleanupOldAuditLogsJob>(
    "cleanup-old-audit-logs",
    job => job.Execute(),
    Cron.Daily(2, 0)  // Every day at 2:00 AM
);
```

### 4. Job Continuations

Chain jobs together (execute after parent completes).

```csharp
var generateReportJobId = BackgroundJob.Enqueue<GenerateReportJob>(
    job => job.Execute(reportId)
);

BackgroundJob.ContinueJobWith<SendReportEmailJob>(
    generateReportJobId,
    job => job.Execute(reportId, userId)
);
```

---

## 📦 Built-in Jobs

### Email Queue Processor

**File:** `src/CoreEngine.Infrastructure/BackgroundJobs/ProcessEmailQueueJob.cs`

```csharp
public class ProcessEmailQueueJob
{
    private readonly IEmailService _emailService;

    public async Task Execute()
    {
        var pendingEmails = await _emailService.GetPendingEmailsAsync(batchSize: 100);

        foreach (var email in pendingEmails)
        {
            BackgroundJob.Enqueue<SendEmailJob>(job => job.Execute(email.Id));
        }
    }
}

// Schedule every minute
RecurringJob.AddOrUpdate<ProcessEmailQueueJob>(
    "process-email-queue",
    job => job.Execute(),
    Cron.Minutely()
);
```

### Cleanup Old Audit Logs

**File:** `src/CoreEngine.Infrastructure/BackgroundJobs/CleanupOldAuditLogsJob.cs`

```csharp
public class CleanupOldAuditLogsJob
{
    public async Task Execute()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-365);
        var deletedCount = await _context.AuditLogs
            .Where(a => a.CreatedAt < cutoffDate)
            .ExecuteDeleteAsync();

        _logger.LogInformation($"Deleted {deletedCount} old audit logs");
    }
}

// Schedule daily at 2 AM
RecurringJob.AddOrUpdate<CleanupOldAuditLogsJob>(
    "cleanup-old-audit-logs",
    job => job.Execute(),
    Cron.Daily(2, 0)
);
```

### Cleanup Soft-Deleted Records

**File:** `src/CoreEngine.Infrastructure/BackgroundJobs/CleanupSoftDeletedRecordsJob.cs`

```csharp
public class CleanupSoftDeletedRecordsJob
{
    public async Task Execute()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-90);

        // Hard delete records that were soft deleted > 90 days ago
        await _context.Users
            .IgnoreQueryFilters()  // Include soft-deleted
            .Where(u => u.IsDeleted && u.ModifiedAt < cutoffDate)
            .ExecuteDeleteAsync();

        // Repeat for other entities...
    }
}

// Schedule weekly on Sundays at 3 AM
RecurringJob.AddOrUpdate<CleanupSoftDeletedRecordsJob>(
    "cleanup-soft-deleted-records",
    job => job.Execute(),
    Cron.Weekly(DayOfWeek.Sunday, 3, 0)
);
```

---

## 🌐 Hangfire Dashboard

Access the Hangfire dashboard at: `https://localhost:5001/hangfire`

### Dashboard Features

- **Jobs** - View all jobs (succeeded, failed, processing, scheduled)
- **Recurring Jobs** - Manage recurring job schedules
- **Servers** - View active Hangfire servers
- **Retries** - View and retry failed jobs
- **Statistics** - Job success/failure rates

### Screenshots

```
┌──────────────────────────────────────────────────────────────┐
│  CoreEngine Background Jobs                   [Sign Out]     │
├──────────────────────────────────────────────────────────────┤
│  Jobs  Recurring Jobs  Servers  Retries  Statistics          │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Succeeded: 1,247         Failed: 12                  │   │
│  │ Processing: 3            Scheduled: 45               │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
│  Recent Jobs:                                                │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ ✅ SendEmailJob          Succeeded  2 mins ago       │   │
│  │ ✅ GenerateReportJob     Succeeded  5 mins ago       │   │
│  │ ⏳ ProcessEmailQueueJob  Processing 1 sec ago        │   │
│  │ ❌ SendEmailJob          Failed     10 mins ago      │   │
│  └──────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────┘
```

---

## 💻 Usage Examples

### Enqueue Fire-and-Forget Job

```csharp
// In a controller or service
public async Task<IActionResult> CreateReport(CreateReportCommand command)
{
    var reportId = await _mediator.Send(command);

    // Enqueue background job to generate report
    BackgroundJob.Enqueue<GenerateReportJob>(job => job.Execute(reportId));

    return Ok(new { reportId, message = "Report generation started" });
}
```

### Schedule Delayed Job

```csharp
// Send reminder email in 24 hours
public async Task CreateWorkflowTask(Guid userId, Guid taskId)
{
    // Create task
    await _context.WorkflowTasks.AddAsync(new WorkflowTask { ... });
    await _context.SaveChangesAsync();

    // Schedule reminder email
    BackgroundJob.Schedule<SendTaskReminderJob>(
        job => job.Execute(userId, taskId),
        TimeSpan.FromHours(24)
    );
}
```

### Register Recurring Job

```csharp
// In Program.cs or a startup service
public void RegisterRecurringJobs()
{
    // Process email queue every minute
    RecurringJob.AddOrUpdate<ProcessEmailQueueJob>(
        "process-email-queue",
        job => job.Execute(),
        Cron.Minutely()
    );

    // Generate daily reports at 6 AM
    RecurringJob.AddOrUpdate<GenerateDailyReportsJob>(
        "generate-daily-reports",
        job => job.Execute(),
        Cron.Daily(6, 0)
    );

    // Cleanup old data weekly on Sundays at 3 AM
    RecurringJob.AddOrUpdate<CleanupOldDataJob>(
        "cleanup-old-data",
        job => job.Execute(),
        Cron.Weekly(DayOfWeek.Sunday, 3, 0)
    );
}
```

---

## 🔄 Job Retry Strategy

### Automatic Retries

```csharp
[AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 3600 })]
public class SendEmailJob
{
    public async Task Execute(Guid emailId)
    {
        // Retry 3 times with delays: 1 min, 5 min, 1 hour
        await _emailService.SendAsync(emailId);
    }
}
```

### Manual Retry

```csharp
// In Hangfire dashboard, click "Retry" button on failed job
// Or programmatically:
BackgroundJob.Requeue(jobId);
```

---

## 🎯 Job Queues (Priority)

### Queue Configuration

```csharp
builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { "critical", "default", "low" };
    options.WorkerCount = 10;  // 10 concurrent jobs
});
```

### Enqueue to Specific Queue

```csharp
// Critical queue (processed first)
BackgroundJob.Enqueue<SendPasswordResetEmailJob>(
    job => job.Execute(userId),
    new EnqueuedState("critical")
);

// Low priority queue (processed last)
BackgroundJob.Enqueue<CleanupTempFilesJob>(
    job => job.Execute(),
    new EnqueuedState("low")
);
```

---

## 🔐 Permissions

- **BackgroundJob.View** - Access Hangfire dashboard
- **BackgroundJob.Manage** - Retry/delete jobs, trigger recurring jobs

**Default Roles:**
- **SuperAdmin**: BackgroundJob.View, BackgroundJob.Manage
- **Admin**: BackgroundJob.View
- **User**: None

---

## 📊 Cron Schedule Examples

```csharp
// Every minute
Cron.Minutely()

// Every hour
Cron.Hourly()

// Daily at 2:00 AM
Cron.Daily(2, 0)

// Weekly on Sundays at 3:00 AM
Cron.Weekly(DayOfWeek.Sunday, 3, 0)

// Monthly on 1st day at midnight
Cron.Monthly(1, 0, 0)

// Every weekday at 9:00 AM
Cron.WeekdayAt(9, 0)

// Custom cron expression
"0 */15 * * * *"  // Every 15 minutes
```

---

## 🚀 Performance Tuning

### Worker Count

```csharp
options.WorkerCount = Environment.ProcessorCount * 2;  // 2x CPU cores
```

### Batch Processing

```csharp
public async Task ProcessLargeBatch()
{
    var totalRecords = await _context.Records.CountAsync();
    var batchSize = 1000;

    for (int i = 0; i < totalRecords; i += batchSize)
    {
        BackgroundJob.Enqueue<ProcessBatchJob>(
            job => job.Execute(i, batchSize)
        );
    }
}
```

### SQL Server Optimization

```csharp
new SqlServerStorageOptions
{
    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
    QueuePollInterval = TimeSpan.Zero,  // Instant polling
    UseRecommendedIsolationLevel = true,
    DisableGlobalLocks = true  // Better performance
}
```

---

## 🧪 Testing Background Jobs

### Unit Test

```csharp
[Fact]
public async Task SendEmailJob_ShouldSendEmail()
{
    // Arrange
    var emailId = Guid.NewGuid();
    var job = new SendEmailJob(_emailService);

    // Act
    await job.Execute(emailId);

    // Assert
    _emailService.Verify(x => x.SendAsync(emailId), Times.Once);
}
```

### Integration Test

```csharp
[Fact]
public async Task EnqueueJob_ShouldProcessSuccessfully()
{
    // Arrange
    var jobId = BackgroundJob.Enqueue<SendEmailJob>(job => job.Execute(emailId));

    // Wait for job to process
    await Task.Delay(5000);

    // Assert
    var jobStatus = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
    Assert.Equal("Succeeded", jobStatus.History[0].StateName);
}
```

---

## 🚨 Important Notes

1. **Job Serialization** - Job arguments must be serializable (use primitives or DTOs)
2. **Tenant Context** - Inject `ITenantContext` to access current tenant in jobs
3. **Idempotency** - Design jobs to be safely retried (no duplicate side effects)
4. **Long-Running Jobs** - Break into smaller batches to avoid timeouts
5. **Dashboard Security** - Always protect Hangfire dashboard with authentication

---

## 📚 Related Documentation

- [Email Queue Module](email-queue.md) - Email sending jobs
- [Audit Logging Module](audit.md) - Cleanup jobs
- [SignalR Module](signalr.md) - Real-time job notifications

---

**[← Back to Modules](../MODULES.md)** | **[Next: Email Templates →](email-templates.md)**
