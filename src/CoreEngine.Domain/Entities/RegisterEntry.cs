using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class RegisterEntry : TenantScopedEntity
{
    public Guid StatutoryRegisterId { get; set; }
    public Guid MineSiteId { get; set; }
    public int EntryNumber { get; set; }
    public DateTime EntryDate { get; set; }
    public Guid? ShiftInstanceId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string Subject { get; set; } = default!;
    public string Details { get; set; } = default!;
    public string ReportedBy { get; set; } = default!;
    public string? WitnessName { get; set; }
    public string? ActionTaken { get; set; }
    public DateTime? ActionDueDate { get; set; }
    public DateTime? ActionCompletedDate { get; set; }
    public string Status { get; set; } = "Open"; // Open, ActionRequired, Closed, Amended
    public Guid? AmendmentOfEntryId { get; set; }
    public string? AmendmentReason { get; set; }

    // Navigation
    public StatutoryRegister StatutoryRegister { get; set; } = default!;
    public MineSite MineSite { get; set; } = default!;
    public ShiftInstance? ShiftInstance { get; set; }
    public MineArea? MineArea { get; set; }
    public RegisterEntry? AmendmentOfEntry { get; set; }
    public ICollection<RegisterEntry> Amendments { get; set; } = new List<RegisterEntry>();
}
