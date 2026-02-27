using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class WorkflowTask : TenantScopedEntity
{
    public Guid WorkflowInstanceId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty; // "Approval", "Review", "Notification"
    public Guid AssignedToUserId { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected", "Completed"
    public int Priority { get; set; } = 2; // 1=Low, 2=Medium, 3=High
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CompletedByUserId { get; set; }
    public string? Comments { get; set; }
    public string DataJson { get; set; } = "{}"; // Task-specific data

    // Navigation
    public WorkflowInstance WorkflowInstance { get; set; } = default!;
    public User AssignedToUser { get; set; } = default!;
    public User? CompletedByUser { get; set; }
}
