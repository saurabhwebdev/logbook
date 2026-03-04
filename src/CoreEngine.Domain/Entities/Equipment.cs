using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class Equipment : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string AssetNumber { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Category { get; set; } = default!; // Vehicle, HeavyMachinery, Drill, Crusher, Conveyor, Pump, Electrical, Ventilation, Safety, Other
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public int? YearOfManufacture { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchaseCost { get; set; }
    public string Status { get; set; } = "Operational"; // Operational, UnderMaintenance, Breakdown, Decommissioned, Standby
    public string? Location { get; set; }
    public string? OperatorName { get; set; }
    public double? HoursOperated { get; set; }
    public double? NextServiceHours { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public DateTime? LastServiceDate { get; set; }
    public string? WarrantyInfo { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
}
