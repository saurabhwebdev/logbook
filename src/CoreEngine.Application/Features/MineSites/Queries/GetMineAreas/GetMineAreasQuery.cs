using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.MineSites.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Queries.GetMineAreas;

public record GetMineAreasQuery(Guid MineSiteId) : IRequest<IReadOnlyList<MineAreaDto>>;

public class GetMineAreasQueryHandler : IRequestHandler<GetMineAreasQuery, IReadOnlyList<MineAreaDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMineAreasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MineAreaDto>> Handle(GetMineAreasQuery request, CancellationToken cancellationToken)
    {
        var areas = await _context.MineAreas
            .AsNoTracking()
            .Where(a => a.MineSiteId == request.MineSiteId)
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.Name)
            .Select(a => new MineAreaDto(
                a.Id,
                a.MineSiteId,
                a.MineSite.Name,
                a.Name,
                a.Code,
                a.AreaType,
                a.Description,
                a.Elevation,
                a.IsActive,
                a.ParentAreaId,
                a.ParentArea != null ? a.ParentArea.Name : null,
                a.SortOrder,
                a.CreatedAt,
                a.ChildAreas.Count
            ))
            .ToListAsync(cancellationToken);

        return areas;
    }
}
