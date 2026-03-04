using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class StatutoryRegister : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string RegisterType { get; set; } = default!; // Accident, DangerousOccurrence, PersonEntry, Explosives, MachineBreakdown, Inspection, WorkmenPresence, Ventilation, TimberSupply, Custom
    public string? Description { get; set; }
    public string Jurisdiction { get; set; } = default!;
    public bool IsRequired { get; set; } = true;
    public int RetentionYears { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public ICollection<RegisterEntry> Entries { get; set; } = new List<RegisterEntry>();
}
