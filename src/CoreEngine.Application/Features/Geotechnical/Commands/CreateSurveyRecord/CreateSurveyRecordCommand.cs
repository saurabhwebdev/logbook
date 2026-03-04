using MediatR;

namespace CoreEngine.Application.Features.Geotechnical.Commands.CreateSurveyRecord;

public record CreateSurveyRecordCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
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
    string? Notes
) : IRequest<Guid>;
