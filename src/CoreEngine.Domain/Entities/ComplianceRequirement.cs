using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ComplianceRequirement : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Jurisdiction { get; set; } = default!;
    public string Category { get; set; } = default!; // Safety, Environmental, Mining, Labor, Health, Emergency, Reporting, Other
    public string Description { get; set; } = default!;
    public string? RegulatoryBody { get; set; }
    public string? ReferenceDocument { get; set; }
    public string Frequency { get; set; } = "AsRequired"; // Daily, Weekly, Monthly, Quarterly, SemiAnnually, Annually, AsRequired
    public DateTime? DueDate { get; set; }
    public DateTime? LastCompletedDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string? ResponsibleRole { get; set; }
    public string Status { get; set; } = "Pending"; // Compliant, NonCompliant, PartiallyCompliant, Pending, Overdue, NotApplicable
    public string Priority { get; set; } = "Medium"; // Critical, High, Medium, Low
    public string? PenaltyForNonCompliance { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public ICollection<ComplianceAudit> Audits { get; set; } = new List<ComplianceAudit>();
}
