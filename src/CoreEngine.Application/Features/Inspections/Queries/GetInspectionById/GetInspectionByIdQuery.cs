using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Inspections.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Queries.GetInspectionById;

public record GetInspectionByIdQuery(Guid Id) : IRequest<InspectionDto>;

public class GetInspectionByIdQueryHandler : IRequestHandler<GetInspectionByIdQuery, InspectionDto>
{
    private readonly IApplicationDbContext _context;
    public GetInspectionByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<InspectionDto> Handle(GetInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Inspections.AsNoTracking()
            .Include(e => e.InspectionTemplate)
            .Include(e => e.MineSite)
            .Include(e => e.MineArea)
            .Where(e => e.Id == request.Id)
            .Select(e => new InspectionDto(
                e.Id, e.InspectionTemplateId, e.InspectionTemplate.Name,
                e.MineSiteId, e.MineSite.Name, e.MineAreaId, e.MineArea != null ? e.MineArea.Name : null,
                e.InspectionNumber, e.Title, e.ScheduledDate, e.CompletedDate,
                e.InspectorName, e.InspectorRole, e.Status, e.OverallRating, e.Summary,
                e.ChecklistResponsesJson, e.WeatherConditions, e.PersonnelPresent,
                e.SignedOffBy, e.SignedOffAt, e.Findings.Count, e.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Inspection {request.Id} not found.");
    }
}
