using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ProductionLog : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public Guid? ShiftInstanceId { get; set; }
    public string LogNumber { get; set; } = default!;
    public DateTime Date { get; set; }
    public string? ShiftName { get; set; }
    public string Material { get; set; } = default!; // Ore, Waste, Topsoil, Overburden, Coal, Other
    public string? SourceLocation { get; set; }
    public string? DestinationLocation { get; set; }
    public decimal QuantityTonnes { get; set; }
    public decimal? QuantityBCM { get; set; }
    public string? EquipmentUsed { get; set; }
    public string? OperatorName { get; set; }
    public double? HaulingDistance { get; set; }
    public int? LoadCount { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Verified, Approved
    public string? Notes { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
