using MediatR;

namespace CoreEngine.Application.Features.Environmental.Commands.UpdateEnvironmentalReading;

public record UpdateEnvironmentalReadingCommand(
    Guid Id,
    string ReadingType,
    string Parameter,
    decimal Value,
    string Unit,
    decimal? ThresholdMin,
    decimal? ThresholdMax,
    bool IsExceedance,
    DateTime ReadingDateTime,
    string? MonitoringStation,
    string? InstrumentUsed,
    DateTime? CalibratedDate,
    string RecordedBy,
    string? WeatherConditions,
    string? Notes,
    string Status
) : IRequest;
