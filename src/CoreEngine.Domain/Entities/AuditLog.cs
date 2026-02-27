namespace CoreEngine.Domain.Entities;

/// <summary>
/// Standalone audit log entity. Not inheriting BaseEntity
/// because it IS the audit record — no soft delete, no CreatedBy/ModifiedBy.
/// Uses bigint IDENTITY for high-write performance.
/// </summary>
public class AuditLog
{
    public long Id { get; set; }
    public Guid? TenantId { get; set; }
    public string? UserId { get; set; }
    public string Action { get; set; } = default!;
    public string EntityName { get; set; } = default!;
    public string EntityId { get; set; } = default!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}
