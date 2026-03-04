namespace CoreEngine.Application.Features.SafetyIncidents.DTOs;

public record IncidentInvestigationDto(
    Guid Id,
    Guid SafetyIncidentId,
    string IncidentTitle,
    string InvestigatorName,
    DateTime InvestigationDate,
    string Methodology,
    string Findings,
    string? RootCauseAnalysis,
    string? Recommendations,
    string? PreventiveMeasures,
    string? EvidenceReferences,
    string Status,
    DateTime CreatedAt
);
