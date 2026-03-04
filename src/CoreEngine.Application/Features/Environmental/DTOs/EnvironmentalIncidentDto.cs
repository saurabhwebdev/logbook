namespace CoreEngine.Application.Features.Environmental.DTOs;

public record EnvironmentalIncidentDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    string IncidentNumber,
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
    string? ClosureNotes,
    DateTime CreatedAt
);
