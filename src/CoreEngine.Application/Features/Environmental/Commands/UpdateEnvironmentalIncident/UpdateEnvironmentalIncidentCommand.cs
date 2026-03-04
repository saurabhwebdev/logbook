using MediatR;

namespace CoreEngine.Application.Features.Environmental.Commands.UpdateEnvironmentalIncident;

public record UpdateEnvironmentalIncidentCommand(
    Guid Id,
    string Title,
    string IncidentType,
    string Severity,
    DateTime OccurredAt,
    string Location,
    string Description,
    string? ImpactAssessment,
    string? ContainmentActions,
    string? RemediationPlan,
    string ReportedBy,
    bool NotifiedAuthority,
    string? AuthorityReference,
    string Status,
    DateTime? ClosedAt,
    string? ClosureNotes
) : IRequest;
