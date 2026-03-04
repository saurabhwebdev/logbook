namespace CoreEngine.Application.Features.Production.DTOs;

public record DispatchRecordDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    string DispatchNumber,
    DateTime Date,
    string VehicleNumber,
    string? DriverName,
    string Material,
    string SourceLocation,
    string DestinationLocation,
    string? WeighbridgeTicketNumber,
    decimal? GrossWeight,
    decimal? TareWeight,
    decimal? NetWeight,
    string Unit,
    DateTime? DepartureTime,
    DateTime? ArrivalTime,
    string Status,
    string? Notes,
    DateTime CreatedAt
);
