namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class StateDefinition : TenantScopedEntity
{
    public string EntityType { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public bool IsInitial { get; set; }
    public bool IsFinal { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
}
