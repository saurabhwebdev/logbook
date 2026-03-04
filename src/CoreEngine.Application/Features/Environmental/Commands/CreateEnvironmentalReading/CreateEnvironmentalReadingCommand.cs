using MediatR;

namespace CoreEngine.Application.Features.Environmental.Commands.CreateEnvironmentalReading;

public record CreateEnvironmentalReadingCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
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
    string? Notes
) : IRequest<Guid>;
