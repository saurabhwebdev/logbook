using MediatR;

namespace CoreEngine.Application.Features.Ventilation.Commands.CreateVentilationReading;

public record CreateVentilationReadingCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
    string LocationDescription,
    decimal? AirflowVelocity,
    decimal? AirflowVolume,
    decimal? Temperature,
    decimal? Humidity,
    decimal? BarometricPressure,
    DateTime ReadingDateTime,
    string RecordedBy,
    string? InstrumentUsed,
    string? DoorStatus,
    string? FanStatus,
    string? Notes
) : IRequest<Guid>;
