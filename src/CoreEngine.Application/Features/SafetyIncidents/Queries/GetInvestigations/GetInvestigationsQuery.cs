using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.SafetyIncidents.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.SafetyIncidents.Queries.GetInvestigations;

public record GetInvestigationsQuery(Guid SafetyIncidentId) : IRequest<IReadOnlyList<IncidentInvestigationDto>>;

public class GetInvestigationsQueryHandler : IRequestHandler<GetInvestigationsQuery, IReadOnlyList<IncidentInvestigationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInvestigationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<IncidentInvestigationDto>> Handle(GetInvestigationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.IncidentInvestigations
            .AsNoTracking()
            .Where(i => i.SafetyIncidentId == request.SafetyIncidentId)
            .OrderByDescending(i => i.InvestigationDate)
            .Select(i => new IncidentInvestigationDto(
                i.Id,
                i.SafetyIncidentId,
                i.SafetyIncident.Title,
                i.InvestigatorName,
                i.InvestigationDate,
                i.Methodology,
                i.Findings,
                i.RootCauseAnalysis,
                i.Recommendations,
                i.PreventiveMeasures,
                i.EvidenceReferences,
                i.Status,
                i.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
