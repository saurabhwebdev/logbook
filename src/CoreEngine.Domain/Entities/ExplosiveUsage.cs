using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ExplosiveUsage : TenantScopedEntity
{
    public Guid BlastEventId { get; set; }
    public string ExplosiveName { get; set; } = default!;
    public string Type { get; set; } = default!; // Emulsion, ANFO, Dynamite, Detonator, Booster, Other
    public string? BatchNumber { get; set; }
    public decimal QuantityIssued { get; set; }
    public decimal QuantityUsed { get; set; }
    public decimal QuantityReturned { get; set; }
    public string Unit { get; set; } = "kg"; // kg, pieces, meters
    public string? MagazineSource { get; set; }
    public string? IssuedBy { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public BlastEvent BlastEvent { get; set; } = default!;
}
