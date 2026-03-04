namespace CoreEngine.Application.Features.Geotechnical.DTOs;

public record GeotechnicalAssessmentDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    Guid? MineAreaId,
    string? MineAreaName,
    string AssessmentNumber,
    string Title,
    string AssessmentType,
    DateTime Date,
    string AssessorName,
    string Location,
    decimal? RockMassRating,
    decimal? SlopeAngle,
    decimal? WaterTableDepth,
    string? GroundCondition,
    string StabilityStatus,
    string? RecommendedActions,
    bool MonitoringRequired,
    DateTime? NextAssessmentDate,
    string? Notes,
    string Status,
    DateTime CreatedAt
);
