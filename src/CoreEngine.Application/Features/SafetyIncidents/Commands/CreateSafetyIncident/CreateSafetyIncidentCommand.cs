using MediatR;

namespace CoreEngine.Application.Features.SafetyIncidents.Commands.CreateSafetyIncident;

public record CreateSafetyIncidentCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
    string Title,
    string IncidentType,
    string? Severity,
    DateTime IncidentDateTime,
    string Location,
    string Description,
    string? ImmediateActions,
    string ReportedBy,
    string? InjuredPersonName,
    string? InjuredPersonRole,
    string? InjuryType,
    string? BodyPartAffected,
    int? LostTimeDays,
    bool? IsReportable,
    string? RegulatoryReference,
    string? WitnessNames,
    string? RootCause,
    string? ContributingFactors,
    string? CorrectiveActions,
    DateTime? CorrectiveActionDueDate
) : IRequest<Guid>;
