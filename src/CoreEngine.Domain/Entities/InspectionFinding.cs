using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class InspectionFinding : TenantScopedEntity
{
    public Guid InspectionId { get; set; }
    public string FindingNumber { get; set; } = default!;
    public string Category { get; set; } = default!; // Safety, Environmental, Equipment, Housekeeping, PPE, Procedure, Structural, Other
    public string Severity { get; set; } = "Low"; // Critical, High, Medium, Low
    public string Description { get; set; } = default!;
    public string? Location { get; set; }
    public string? RecommendedAction { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? ActionDueDate { get; set; }
    public DateTime? ActionCompletedDate { get; set; }
    public string Status { get; set; } = "Open"; // Open, InProgress, Completed, Overdue, Closed
    public string? ClosureNotes { get; set; }

    // Navigation
    public Inspection Inspection { get; set; } = default!;
}
