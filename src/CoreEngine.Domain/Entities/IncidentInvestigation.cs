using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class IncidentInvestigation : TenantScopedEntity
{
    public Guid SafetyIncidentId { get; set; }
    public string InvestigatorName { get; set; } = default!;
    public DateTime InvestigationDate { get; set; }
    public string Methodology { get; set; } = default!; // FiveWhys, FishboneDiagram, TapRooT, BowTie, FaultTreeAnalysis, IncidentCauseAnalysis, Other
    public string Findings { get; set; } = default!;
    public string? RootCauseAnalysis { get; set; }
    public string? Recommendations { get; set; }
    public string? PreventiveMeasures { get; set; }
    public string? EvidenceReferences { get; set; }
    public string Status { get; set; } = "InProgress"; // InProgress, Completed, ReviewPending

    // Navigation
    public SafetyIncident SafetyIncident { get; set; } = default!;
}
