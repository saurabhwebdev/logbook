using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Compliance.Commands.CreateComplianceAudit;

public class CreateComplianceAuditCommandHandler : IRequestHandler<CreateComplianceAuditCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateComplianceAuditCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateComplianceAuditCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.ComplianceAudits.CountAsync(cancellationToken) + 1;
        var auditNumber = $"CA-{count:D5}";

        var audit = new ComplianceAudit
        {
            Id = Guid.NewGuid(),
            ComplianceRequirementId = request.ComplianceRequirementId,
            AuditNumber = auditNumber,
            AuditDate = request.AuditDate,
            AuditorName = request.AuditorName,
            AuditType = request.AuditType,
            Findings = request.Findings,
            ComplianceStatus = request.ComplianceStatus,
            CorrectiveActions = request.CorrectiveActions,
            ActionDueDate = request.ActionDueDate,
            EvidenceReferences = request.EvidenceReferences,
            Status = "Open",
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ComplianceAudits.Add(audit);
        await _context.SaveChangesAsync(cancellationToken);

        return audit.Id;
    }
}
