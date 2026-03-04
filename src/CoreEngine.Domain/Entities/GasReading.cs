using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class GasReading : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string ReadingNumber { get; set; } = default!;
    public string GasType { get; set; } = default!; // Methane, CarbonMonoxide, CarbonDioxide, NitrogenDioxide, HydrogenSulfide, Oxygen, SulfurDioxide, Other
    public decimal Concentration { get; set; }
    public string Unit { get; set; } = default!; // ppm, percent, mgm3
    public decimal? ThresholdTWA { get; set; }
    public decimal? ThresholdSTEL { get; set; }
    public decimal? ThresholdCeiling { get; set; }
    public bool IsExceedance { get; set; }
    public string LocationDescription { get; set; } = default!;
    public DateTime ReadingDateTime { get; set; }
    public string RecordedBy { get; set; } = default!;
    public string? InstrumentId { get; set; }
    public DateTime? CalibrationDate { get; set; }
    public string? ActionTaken { get; set; }
    public string Status { get; set; } = "Normal"; // Normal, Warning, Alarm, Evacuation
    public string? Notes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
