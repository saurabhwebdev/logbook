using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ShiftDefinition : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int ShiftOrder { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public ICollection<ShiftInstance> ShiftInstances { get; set; } = new List<ShiftInstance>();
}
