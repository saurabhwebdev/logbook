using System.Text.Json;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CoreEngine.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public AuditableEntityInterceptor(
        ITenantContext tenantContext,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not ApplicationDbContext context)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Skip AuditLog entities to prevent infinite recursion
            if (entry.Entity is AuditLog)
                continue;

            // Skip RefreshToken entities (too noisy)
            if (entry.Entity is RefreshToken)
                continue;

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            var auditEntry = new AuditEntry
            {
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State switch
                {
                    EntityState.Added => "Create",
                    EntityState.Modified => "Update",
                    EntityState.Deleted => "Delete",
                    _ => "Unknown"
                },
                TenantId = _tenantContext.TenantId == Guid.Empty ? null : _tenantContext.TenantId,
                UserId = _currentUserService.UserId,
                IpAddress = _currentUserService.IpAddress,
                Timestamp = _dateTimeService.UtcNow
            };

            // Try to get EntityId from known properties
            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
            auditEntry.EntityId = idProperty?.CurrentValue?.ToString() ?? "Unknown";

            switch (entry.State)
            {
                case EntityState.Added:
                {
                    var newValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        newValues[property.Metadata.Name] = property.CurrentValue;
                    }
                    auditEntry.NewValues = JsonSerializer.Serialize(newValues);
                    // For newly added entities, the Id may be set later (e.g., by SaveChangesAsync override).
                    // We capture a temporary entry and update it after save if needed.
                    auditEntry.HasTemporaryId = idProperty?.IsTemporary ?? false;
                    auditEntry.IdProperty = idProperty;
                    break;
                }
                case EntityState.Modified:
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            oldValues[property.Metadata.Name] = property.OriginalValue;
                            newValues[property.Metadata.Name] = property.CurrentValue;
                        }
                    }
                    auditEntry.OldValues = JsonSerializer.Serialize(oldValues);
                    auditEntry.NewValues = JsonSerializer.Serialize(newValues);
                    break;
                }
                case EntityState.Deleted:
                {
                    var oldValues = new Dictionary<string, object?>();
                    foreach (var property in entry.Properties)
                    {
                        oldValues[property.Metadata.Name] = property.OriginalValue;
                    }
                    auditEntry.OldValues = JsonSerializer.Serialize(oldValues);
                    break;
                }
            }

            auditEntries.Add(auditEntry);
        }

        // Add audit logs to the context
        foreach (var auditEntry in auditEntries)
        {
            context.AuditLogs.Add(new AuditLog
            {
                TenantId = auditEntry.TenantId,
                UserId = auditEntry.UserId,
                Action = auditEntry.Action,
                EntityName = auditEntry.EntityName,
                EntityId = auditEntry.EntityId,
                OldValues = auditEntry.OldValues,
                NewValues = auditEntry.NewValues,
                IpAddress = auditEntry.IpAddress,
                Timestamp = auditEntry.Timestamp
            });
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Internal helper to hold audit entry data before creating the AuditLog entity.
    /// </summary>
    private class AuditEntry
    {
        public string EntityName { get; set; } = default!;
        public string EntityId { get; set; } = default!;
        public string Action { get; set; } = default!;
        public Guid? TenantId { get; set; }
        public string? UserId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasTemporaryId { get; set; }
        public Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry? IdProperty { get; set; }
    }
}
