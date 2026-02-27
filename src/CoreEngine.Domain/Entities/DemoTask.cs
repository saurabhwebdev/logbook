namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class DemoTask : TenantScopedEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CurrentState { get; set; } = "Draft";
    public string? AssignedTo { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical
}
