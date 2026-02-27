namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class StateTransitionLog : TenantScopedEntity
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string FromState { get; set; } = string.Empty;
    public string ToState { get; set; } = string.Empty;
    public string TriggerName { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public string? Comments { get; set; }
    public DateTime TransitionedAt { get; set; }
}
