namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class WebhookSubscription : TenantScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public string EndpointUrl { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty; // HMAC signing secret
    public string EventTypes { get; set; } = string.Empty; // Comma-separated event types
    public bool IsActive { get; set; } = true;
    public DateTime? LastTriggeredAt { get; set; }
    public int FailureCount { get; set; }
}
