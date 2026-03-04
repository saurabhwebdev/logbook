using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Inspections.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Queries.GetInspectionTemplates;

public record GetInspectionTemplatesQuery(string? Category, bool? IsActive) : IRequest<IReadOnlyList<InspectionTemplateDto>>;

public class GetInspectionTemplatesQueryHandler : IRequestHandler<GetInspectionTemplatesQuery, IReadOnlyList<InspectionTemplateDto>>
{
    private readonly IApplicationDbContext _context;
    public GetInspectionTemplatesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<InspectionTemplateDto>> Handle(GetInspectionTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InspectionTemplates.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(e => e.Category == request.Category);
        if (request.IsActive.HasValue)
            query = query.Where(e => e.IsActive == request.IsActive.Value);

        return await query.OrderBy(e => e.SortOrder).ThenBy(e => e.Name)
            .Select(e => new InspectionTemplateDto(
                e.Id, e.Name, e.Code, e.Category, e.Description, e.ChecklistJson,
                e.Frequency, e.IsActive, e.SortOrder, e.Inspections.Count, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
