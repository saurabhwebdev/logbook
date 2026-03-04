namespace CoreEngine.Application.Features.Ventilation.DTOs;

public record GasReadingDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    Guid? MineAreaId,
    string? MineAreaName,
    string ReadingNumber,
    string GasType,
    decimal Concentration,
    string Unit,
    decimal? ThresholdTWA,
    decimal? ThresholdSTEL,
    decimal? ThresholdCeiling,
    bool IsExceedance,
    string LocationDescription,
    DateTime ReadingDateTime,
    string RecordedBy,
    string? InstrumentId,
    DateTime? CalibrationDate,
    string? ActionTaken,
    string Status,
    string? Notes,
    DateTime CreatedAt
);
