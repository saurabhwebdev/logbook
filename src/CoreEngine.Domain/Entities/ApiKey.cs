namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class ApiKey : TenantScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty; // BCrypt hash of the actual key
    public string KeyPrefix { get; set; } = string.Empty; // First 8 chars for display (e.g. "ce_1a2b...")
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastUsedAt { get; set; }
    public string? Scopes { get; set; } // Comma-separated permission scopes
}
