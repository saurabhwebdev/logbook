using MediatR;

namespace CoreEngine.Application.Features.Compliance.Commands.CreateComplianceAudit;

public record CreateComplianceAuditCommand(
    Guid ComplianceRequirementId,
    DateTime AuditDate,
    string AuditorName,
    string AuditType,
    string Findings,
    string ComplianceStatus,
    string? CorrectiveActions,
    DateTime? ActionDueDate,
    string? EvidenceReferences,
    string? Notes
) : IRequest<Guid>;
