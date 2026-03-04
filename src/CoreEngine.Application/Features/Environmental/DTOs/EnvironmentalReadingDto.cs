namespace CoreEngine.Application.Features.Environmental.DTOs;

public record EnvironmentalReadingDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    Guid? MineAreaId,
    string? MineAreaName,
    string ReadingNumber,
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
    string Status,
    DateTime CreatedAt
);
