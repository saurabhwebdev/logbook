namespace CoreEngine.Application.Features.Compliance.DTOs;

public record ComplianceAuditDto(
    Guid Id,
    Guid ComplianceRequirementId,
    string RequirementTitle,
    string AuditNumber,
    DateTime AuditDate,
    string AuditorName,
    string AuditType,
    string Findings,
    string ComplianceStatus,
    string? CorrectiveActions,
    DateTime? ActionDueDate,
    DateTime? ActionCompletedDate,
    string? EvidenceReferences,
    string Status,
    string? Notes,
    DateTime CreatedAt
);
