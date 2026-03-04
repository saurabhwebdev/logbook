using MediatR;

namespace CoreEngine.Application.Features.SafetyIncidents.Commands.UpdateSafetyIncident;

public record UpdateSafetyIncidentCommand(
    Guid Id,
    string Title,
    string IncidentType,
    string Severity,
    DateTime IncidentDateTime,
    string Location,
    string Description,
    string? ImmediateActions,
    string? InjuredPersonName,
    string? InjuredPersonRole,
    string? InjuryType,
    string? BodyPartAffected,
    int? LostTimeDays,
    bool IsReportable,
    string? RegulatoryReference,
    string? WitnessNames,
    string? RootCause,
    string? ContributingFactors,
    string? CorrectiveActions,
    DateTime? CorrectiveActionDueDate,
    DateTime? CorrectiveActionCompletedDate,
    string Status
) : IRequest;
