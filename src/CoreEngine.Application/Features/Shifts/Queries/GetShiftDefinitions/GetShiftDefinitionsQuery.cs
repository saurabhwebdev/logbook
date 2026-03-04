using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Shifts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Queries.GetShiftDefinitions;

public record GetShiftDefinitionsQuery(Guid MineSiteId) : IRequest<IReadOnlyList<ShiftDefinitionDto>>;

public class GetShiftDefinitionsQueryHandler : IRequestHandler<GetShiftDefinitionsQuery, IReadOnlyList<ShiftDefinitionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetShiftDefinitionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ShiftDefinitionDto>> Handle(GetShiftDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var definitions = await _context.ShiftDefinitions
            .AsNoTracking()
            .Where(s => s.MineSiteId == request.MineSiteId && !s.IsDeleted)
            .OrderBy(s => s.ShiftOrder)
            .ThenBy(s => s.Name)
            .Select(s => new ShiftDefinitionDto(
                s.Id,
                s.MineSiteId,
                s.MineSite.Name,
                s.Name,
                s.Code,
                s.StartTime.ToString(@"hh\:mm"),
                s.EndTime.ToString(@"hh\:mm"),
                s.ShiftOrder,
                s.Color,
                s.IsActive,
                s.CreatedAt,
                s.ShiftInstances.Count
            ))
            .ToListAsync(cancellationToken);

        return definitions;
    }
}
