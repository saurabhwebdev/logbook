using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Inspections.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Queries.GetInspections;

public record GetInspectionsQuery(Guid? MineSiteId, Guid? TemplateId, string? Status) : IRequest<IReadOnlyList<InspectionDto>>;

public class GetInspectionsQueryHandler : IRequestHandler<GetInspectionsQuery, IReadOnlyList<InspectionDto>>
{
    private readonly IApplicationDbContext _context;
    public GetInspectionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<InspectionDto>> Handle(GetInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inspections.AsNoTracking()
            .Include(e => e.InspectionTemplate)
            .Include(e => e.MineSite)
            .Include(e => e.MineArea)
            .AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);
        if (request.TemplateId.HasValue)
            query = query.Where(e => e.InspectionTemplateId == request.TemplateId.Value);
        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query.OrderByDescending(e => e.ScheduledDate)
            .Select(e => new InspectionDto(
                e.Id, e.InspectionTemplateId, e.InspectionTemplate.Name,
                e.MineSiteId, e.MineSite.Name, e.MineAreaId, e.MineArea != null ? e.MineArea.Name : null,
                e.InspectionNumber, e.Title, e.ScheduledDate, e.CompletedDate,
                e.InspectorName, e.InspectorRole, e.Status, e.OverallRating, e.Summary,
                e.ChecklistResponsesJson, e.WeatherConditions, e.PersonnelPresent,
                e.SignedOffBy, e.SignedOffAt, e.Findings.Count, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
