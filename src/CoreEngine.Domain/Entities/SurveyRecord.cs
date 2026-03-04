using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class SurveyRecord : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string SurveyNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string SurveyType { get; set; } = default!; // Boundary, Topographic, Underground, Stockpile, Pit, AsBuilt, Monitoring, Other
    public DateTime Date { get; set; }
    public string SurveyorName { get; set; } = default!;
    public string? SurveyorLicense { get; set; }
    public string Location { get; set; } = default!;
    public decimal? Easting { get; set; }
    public decimal? Northing { get; set; }
    public decimal? Elevation { get; set; }
    public string? Datum { get; set; }
    public string? CoordinateSystem { get; set; }
    public string? EquipmentUsed { get; set; }
    public string? Accuracy { get; set; }
    public decimal? VolumeCalculated { get; set; }
    public decimal? AreaCalculated { get; set; }
    public string? Findings { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Reviewed, Approved

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
