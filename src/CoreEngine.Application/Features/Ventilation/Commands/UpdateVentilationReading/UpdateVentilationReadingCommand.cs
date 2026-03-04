using MediatR;

namespace CoreEngine.Application.Features.Ventilation.Commands.UpdateVentilationReading;

public record UpdateVentilationReadingCommand(
    Guid Id,
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
    string VentilationStatus,
    string? Notes
) : IRequest;
