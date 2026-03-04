namespace CoreEngine.Application.Features.Geotechnical.DTOs;

public record SurveyRecordDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    Guid? MineAreaId,
    string? MineAreaName,
    string SurveyNumber,
    string Title,
    string SurveyType,
    DateTime Date,
    string SurveyorName,
    string? SurveyorLicense,
    string Location,
    decimal? Easting,
    decimal? Northing,
    decimal? Elevation,
    string? Datum,
    string? CoordinateSystem,
    string? EquipmentUsed,
    string? Accuracy,
    decimal? VolumeCalculated,
    decimal? AreaCalculated,
    string? Findings,
    string? Notes,
    string Status,
    DateTime CreatedAt
);
