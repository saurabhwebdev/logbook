using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class WorkflowDefinition : TenantScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // "Approval", "Notification", "DataProcessing"
    public string ConfigurationJson { get; set; } = string.Empty; // Workflow steps as JSON
    public bool IsActive { get; set; } = true;
    public int Version { get; set; } = 1;

    // Navigation
    public ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
}
