using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Environmental.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Queries.GetEnvironmentalIncidents;

public record GetEnvironmentalIncidentsQuery(Guid? MineSiteId = null, string? Status = null) : IRequest<IReadOnlyList<EnvironmentalIncidentDto>>;

public class GetEnvironmentalIncidentsQueryHandler : IRequestHandler<GetEnvironmentalIncidentsQuery, IReadOnlyList<EnvironmentalIncidentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEnvironmentalIncidentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<EnvironmentalIncidentDto>> Handle(GetEnvironmentalIncidentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EnvironmentalIncidents.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EnvironmentalIncidentDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.IncidentNumber,
                e.Title,
                e.IncidentType,
                e.Severity,
                e.OccurredAt,
                e.Location,
                e.Description,
                e.ImpactAssessment,
                e.ContainmentActions,
                e.RemediationPlan,
                e.ReportedBy,
                e.NotifiedAuthority,
                e.AuthorityReference,
                e.Status,
                e.ClosedAt,
                e.ClosureNotes,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
