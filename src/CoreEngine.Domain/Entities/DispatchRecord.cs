using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class DispatchRecord : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public string DispatchNumber { get; set; } = default!;
    public DateTime Date { get; set; }
    public string VehicleNumber { get; set; } = default!;
    public string? DriverName { get; set; }
    public string Material { get; set; } = default!;
    public string SourceLocation { get; set; } = default!;
    public string DestinationLocation { get; set; } = default!;
    public string? WeighbridgeTicketNumber { get; set; }
    public decimal? GrossWeight { get; set; }
    public decimal? TareWeight { get; set; }
    public decimal? NetWeight { get; set; }
    public string Unit { get; set; } = "Tonnes"; // Tonnes, CubicMeters
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public string Status { get; set; } = "Loading"; // Loading, InTransit, Delivered, Cancelled
    public string? Notes { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
}
