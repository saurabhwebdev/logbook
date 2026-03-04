using MediatR;

namespace CoreEngine.Application.Features.Production.Commands.CreateDispatchRecord;

public record CreateDispatchRecordCommand(
    Guid MineSiteId,
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
    string? Unit,
    DateTime? DepartureTime,
    DateTime? ArrivalTime,
    string? Notes
) : IRequest<Guid>;
