using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class VentilationReading : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string ReadingNumber { get; set; } = default!;
    public string LocationDescription { get; set; } = default!;
    public decimal? AirflowVelocity { get; set; }
    public decimal? AirflowVolume { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
    public decimal? BarometricPressure { get; set; }
    public DateTime ReadingDateTime { get; set; }
    public string RecordedBy { get; set; } = default!;
    public string? InstrumentUsed { get; set; }
    public string? DoorStatus { get; set; } // Open, Closed, PartiallyOpen
    public string? FanStatus { get; set; } // Running, Stopped, Fault
    public string VentilationStatus { get; set; } = "Normal"; // Normal, Restricted, Inadequate, Critical
    public string? Notes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
}
