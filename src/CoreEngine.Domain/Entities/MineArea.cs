using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class MineArea : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string AreaType { get; set; } = default!; // Pit, Panel, Level, Bench, Stope, Decline, Shaft, ProcessingPlant, Stockpile, WasteDump, TailingsDam, Workshop, Magazine, Office, Other
    public string? Description { get; set; }
    public double? Elevation { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentAreaId { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? ParentArea { get; set; }
    public ICollection<MineArea> ChildAreas { get; set; } = new List<MineArea>();
}
