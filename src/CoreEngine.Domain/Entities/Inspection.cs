using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class Inspection : TenantScopedEntity
{
    public Guid InspectionTemplateId { get; set; }
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string InspectionNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string InspectorName { get; set; } = default!;
    public string? InspectorRole { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Overdue, Cancelled
    public string? OverallRating { get; set; } // Compliant, PartiallyCompliant, NonCompliant, NotApplicable
    public string? Summary { get; set; }
    public string? ChecklistResponsesJson { get; set; } // JSON of responses to checklist items
    public string? WeatherConditions { get; set; }
    public int? PersonnelPresent { get; set; }
    public string? SignedOffBy { get; set; }
    public DateTime? SignedOffAt { get; set; }

    // Navigation
    public InspectionTemplate InspectionTemplate { get; set; } = default!;
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
    public ICollection<InspectionFinding> Findings { get; set; } = new List<InspectionFinding>();
}
