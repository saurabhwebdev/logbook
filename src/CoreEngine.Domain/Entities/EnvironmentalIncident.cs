using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class EnvironmentalIncident : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string IncidentNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string IncidentType { get; set; } = default!; // Spill, Emission, Discharge, Dust, Noise, WaterContamination, LandDegradation, Other
    public string Severity { get; set; } = default!; // Critical, High, Medium, Low
    public DateTime OccurredAt { get; set; }
    public string Location { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImpactAssessment { get; set; }
    public string? ContainmentActions { get; set; }
    public string? RemediationPlan { get; set; }
    public string ReportedBy { get; set; } = default!;
    public bool NotifiedAuthority { get; set; }
    public string? AuthorityReference { get; set; }
    public string Status { get; set; } = "Open"; // Open, Investigating, Remediation, Closed
    public DateTime? ClosedAt { get; set; }
    public string? ClosureNotes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
}
