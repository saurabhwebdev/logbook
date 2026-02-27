using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class WorkflowInstance : TenantScopedEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public string EntityType { get; set; } = string.Empty; // "PurchaseOrder", "LeaveRequest"
    public string EntityId { get; set; } = string.Empty;
    public string Status { get; set; } = "Running"; // "Running", "Completed", "Cancelled"
    public string CurrentStepName { get; set; } = string.Empty;
    public string ContextJson { get; set; } = "{}"; // Workflow variables
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public WorkflowDefinition WorkflowDefinition { get; set; } = default!;
    public ICollection<WorkflowTask> WorkflowTasks { get; set; } = new List<WorkflowTask>();
}
