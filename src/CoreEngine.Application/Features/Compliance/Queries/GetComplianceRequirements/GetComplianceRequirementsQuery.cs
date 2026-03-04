using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Compliance.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Compliance.Queries.GetComplianceRequirements;

public record GetComplianceRequirementsQuery(Guid? MineSiteId = null, string? Status = null, string? Category = null) : IRequest<IReadOnlyList<ComplianceRequirementDto>>;

public class GetComplianceRequirementsQueryHandler : IRequestHandler<GetComplianceRequirementsQuery, IReadOnlyList<ComplianceRequirementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetComplianceRequirementsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ComplianceRequirementDto>> Handle(GetComplianceRequirementsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ComplianceRequirements.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(e => e.Category == request.Category);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new ComplianceRequirementDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.Code,
                e.Title,
                e.Jurisdiction,
                e.Category,
                e.Description,
                e.RegulatoryBody,
                e.ReferenceDocument,
                e.Frequency,
                e.DueDate,
                e.LastCompletedDate,
                e.NextDueDate,
                e.ResponsibleRole,
                e.Status,
                e.Priority,
                e.PenaltyForNonCompliance,
                e.Notes,
                e.IsActive,
                e.Audits.Count,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
