using MediatR;

namespace CoreEngine.Application.Features.Production.Commands.UpdateDispatchRecord;

public record UpdateDispatchRecordCommand(
    Guid Id,
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
    string? Notes
) : IRequest;
