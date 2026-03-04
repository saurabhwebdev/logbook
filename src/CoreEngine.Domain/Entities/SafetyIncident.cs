using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class SafetyIncident : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string IncidentNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string IncidentType { get; set; } = default!; // Injury, NearMiss, PropertyDamage, EnvironmentalRelease, FireExplosion, ElectricalIncident, VehicleIncident, FallOfGround, Other
    public string Severity { get; set; } = "Low"; // Critical, High, Medium, Low
    public DateTime IncidentDateTime { get; set; }
    public string Location { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImmediateActions { get; set; }
    public string ReportedBy { get; set; } = default!;
    public DateTime ReportedAt { get; set; }
    public string? InjuredPersonName { get; set; }
    public string? InjuredPersonRole { get; set; }
    public string? InjuryType { get; set; } // Fatality, LostTimeInjury, MedicalTreatment, FirstAid, NoInjury
    public string? BodyPartAffected { get; set; }
    public int? LostTimeDays { get; set; }
    public bool IsReportable { get; set; }
    public string? RegulatoryReference { get; set; }
    public string? WitnessNames { get; set; }
    public string? RootCause { get; set; }
    public string? ContributingFactors { get; set; }
    public string? CorrectiveActions { get; set; }
    public DateTime? CorrectiveActionDueDate { get; set; }
    public DateTime? CorrectiveActionCompletedDate { get; set; }
    public string Status { get; set; } = "Open"; // Open, UnderInvestigation, ActionRequired, Closed, ReopenedForReview

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
    public ICollection<IncidentInvestigation> Investigations { get; set; } = new List<IncidentInvestigation>();
}
