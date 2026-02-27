namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class StateTransitionDefinition : TenantScopedEntity
{
    public string EntityType { get; set; } = string.Empty;
    public string FromState { get; set; } = string.Empty;
    public string ToState { get; set; } = string.Empty;
    public string TriggerName { get; set; } = string.Empty;
    public string? RequiredPermission { get; set; }
    public string? Description { get; set; }
}
