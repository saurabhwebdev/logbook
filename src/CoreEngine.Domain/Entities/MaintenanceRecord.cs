using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class MaintenanceRecord : TenantScopedEntity
{
    public Guid EquipmentId { get; set; }
    public string WorkOrderNumber { get; set; } = default!;
    public string MaintenanceType { get; set; } = default!; // Preventive, Corrective, Predictive, Emergency, Overhaul, Inspection
    public string Priority { get; set; } = "Medium"; // Critical, High, Medium, Low
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? PerformedBy { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled, Overdue
    public double? DowntimeHours { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public string? PartsUsed { get; set; }
    public string? Findings { get; set; }
    public string? ActionsTaken { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Equipment Equipment { get; set; } = default!;
}
