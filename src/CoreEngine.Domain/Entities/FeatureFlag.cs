namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class FeatureFlag : TenantScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
}
