using MediatR;

namespace CoreEngine.Application.Features.Geotechnical.Commands.UpdateGeotechnicalAssessment;

public record UpdateGeotechnicalAssessmentCommand(
    Guid Id,
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
    string Status
) : IRequest;
