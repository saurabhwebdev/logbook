using MediatR;

namespace CoreEngine.Application.Features.Environmental.Commands.CreateEnvironmentalIncident;

public record CreateEnvironmentalIncidentCommand(
    Guid MineSiteId,
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
    string? AuthorityReference
) : IRequest<Guid>;
