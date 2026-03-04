using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Compliance.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Compliance.Queries.GetComplianceAudits;

public record GetComplianceAuditsQuery(Guid ComplianceRequirementId) : IRequest<IReadOnlyList<ComplianceAuditDto>>;

public class GetComplianceAuditsQueryHandler : IRequestHandler<GetComplianceAuditsQuery, IReadOnlyList<ComplianceAuditDto>>
{
    private readonly IApplicationDbContext _context;

    public GetComplianceAuditsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ComplianceAuditDto>> Handle(GetComplianceAuditsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ComplianceAudits
            .AsNoTracking()
            .Where(e => e.ComplianceRequirementId == request.ComplianceRequirementId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new ComplianceAuditDto(
                e.Id,
                e.ComplianceRequirementId,
                e.ComplianceRequirement.Title,
                e.AuditNumber,
                e.AuditDate,
                e.AuditorName,
                e.AuditType,
                e.Findings,
                e.ComplianceStatus,
                e.CorrectiveActions,
                e.ActionDueDate,
                e.ActionCompletedDate,
                e.EvidenceReferences,
                e.Status,
                e.Notes,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
