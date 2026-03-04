using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class WorkPermit : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string PermitNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string PermitType { get; set; } = default!; // HotWork, ConfinedSpace, WorkingAtHeight, Electrical, Excavation, CraneLifting, Demolition, Other
    public string RequestedBy { get; set; } = default!;
    public DateTime RequestDate { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Location { get; set; } = default!;
    public string WorkDescription { get; set; } = default!;
    public string? HazardsIdentified { get; set; }
    public string? ControlMeasures { get; set; }
    public string? PPERequired { get; set; }
    public string? EmergencyProcedures { get; set; }
    public bool GasTestRequired { get; set; }
    public string? GasTestResults { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Pending, Approved, Active, Completed, Rejected, Expired, Cancelled
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
