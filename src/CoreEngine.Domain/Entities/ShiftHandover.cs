using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ShiftHandover : TenantScopedEntity
{
    public Guid OutgoingShiftInstanceId { get; set; }
    public Guid? IncomingShiftInstanceId { get; set; }
    public Guid MineSiteId { get; set; }
    public DateTime HandoverDateTime { get; set; }
    public string? SafetyIssues { get; set; }
    public string? OngoingOperations { get; set; }
    public string? PendingTasks { get; set; }
    public string? EquipmentStatus { get; set; }
    public string? EnvironmentalConditions { get; set; }
    public string? GeneralRemarks { get; set; }
    public string? HandedOverBy { get; set; }
    public string? ReceivedBy { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Acknowledged
    public DateTime? AcknowledgedAt { get; set; }

    // Navigation
    public ShiftInstance OutgoingShiftInstance { get; set; } = default!;
    public ShiftInstance? IncomingShiftInstance { get; set; }
    public MineSite MineSite { get; set; } = default!;
}
