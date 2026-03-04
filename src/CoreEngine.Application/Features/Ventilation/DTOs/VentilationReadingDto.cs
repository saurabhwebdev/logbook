namespace CoreEngine.Application.Features.Ventilation.DTOs;

public record VentilationReadingDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    Guid? MineAreaId,
    string? MineAreaName,
    string ReadingNumber,
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
    string? Notes,
    DateTime CreatedAt
);
