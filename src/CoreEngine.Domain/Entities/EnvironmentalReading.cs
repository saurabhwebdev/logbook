using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class EnvironmentalReading : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string ReadingNumber { get; set; } = default!;
    public string ReadingType { get; set; } = default!; // DustLevel, NoiseLevel, WaterQuality, AirQuality, GroundVibration, WaterLevel, SoilContamination, BlastVibration, Other
    public string Parameter { get; set; } = default!;
    public decimal Value { get; set; }
    public string Unit { get; set; } = default!;
    public decimal? ThresholdMin { get; set; }
    public decimal? ThresholdMax { get; set; }
    public bool IsExceedance { get; set; }
    public DateTime ReadingDateTime { get; set; }
    public string? MonitoringStation { get; set; }
    public string? InstrumentUsed { get; set; }
    public DateTime? CalibratedDate { get; set; }
    public string RecordedBy { get; set; } = default!;
    public string? WeatherConditions { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Normal"; // Normal, Warning, Critical, Exceedance

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
