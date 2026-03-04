using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class InspectionTemplate : TenantScopedEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Category { get; set; } = default!; // Safety, Environmental, Equipment, Workplace, Regulatory, Electrical, Ventilation, Other
    public string? Description { get; set; }
    public string? ChecklistJson { get; set; } // JSON array of checklist items
    public string Frequency { get; set; } = "Monthly"; // Daily, Weekly, Fortnightly, Monthly, Quarterly, Annually, AdHoc
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    // Navigation
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}
