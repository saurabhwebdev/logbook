namespace CoreEngine.Application.Features.Compliance.DTOs;

public record ComplianceRequirementDto(
    Guid Id,
    Guid MineSiteId,
    string MineSiteName,
    string Code,
    string Title,
    string Jurisdiction,
    string Category,
    string Description,
    string? RegulatoryBody,
    string? ReferenceDocument,
    string Frequency,
    DateTime? DueDate,
    DateTime? LastCompletedDate,
    DateTime? NextDueDate,
    string? ResponsibleRole,
    string Status,
    string Priority,
    string? PenaltyForNonCompliance,
    string? Notes,
    bool IsActive,
    int AuditCount,
    DateTime CreatedAt
);
