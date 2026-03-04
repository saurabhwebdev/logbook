using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class GeotechnicalAssessment : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string AssessmentNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string AssessmentType { get; set; } = default!; // SlopeStability, RockMassClassification, GroundCondition, SubsidenceMonitoring, WaterTable, FoundationAssessment, Other
    public DateTime Date { get; set; }
    public string AssessorName { get; set; } = default!;
    public string Location { get; set; } = default!;
    public decimal? RockMassRating { get; set; }
    public decimal? SlopeAngle { get; set; }
    public decimal? WaterTableDepth { get; set; }
    public string? GroundCondition { get; set; } // Good, Fair, Poor, VeryPoor, Critical
    public string StabilityStatus { get; set; } = "Stable"; // Stable, Marginal, Unstable, Critical
    public string? RecommendedActions { get; set; }
    public bool MonitoringRequired { get; set; }
    public DateTime? NextAssessmentDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Reviewed, Approved

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
