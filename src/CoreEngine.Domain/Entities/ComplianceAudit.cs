using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ComplianceAudit : TenantScopedEntity
{
    public Guid ComplianceRequirementId { get; set; }
    public string AuditNumber { get; set; } = default!;
    public DateTime AuditDate { get; set; }
    public string AuditorName { get; set; } = default!;
    public string AuditType { get; set; } = default!; // Internal, External, Regulatory, SelfAssessment
    public string Findings { get; set; } = default!;
    public string ComplianceStatus { get; set; } = "Compliant"; // Compliant, NonCompliant, PartiallyCompliant, Observation
    public string? CorrectiveActions { get; set; }
    public DateTime? ActionDueDate { get; set; }
    public DateTime? ActionCompletedDate { get; set; }
    public string? EvidenceReferences { get; set; }
    public string Status { get; set; } = "Open"; // Open, InProgress, Closed
    public string? Notes { get; set; }

    // Navigation
    public ComplianceRequirement ComplianceRequirement { get; set; } = default!;
}
