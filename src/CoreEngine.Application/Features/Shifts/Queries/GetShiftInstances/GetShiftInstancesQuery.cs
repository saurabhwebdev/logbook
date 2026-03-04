using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Shifts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Queries.GetShiftInstances;

public record GetShiftInstancesQuery(Guid MineSiteId, DateOnly? FromDate, DateOnly? ToDate) : IRequest<IReadOnlyList<ShiftInstanceDto>>;

public class GetShiftInstancesQueryHandler : IRequestHandler<GetShiftInstancesQuery, IReadOnlyList<ShiftInstanceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetShiftInstancesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ShiftInstanceDto>> Handle(GetShiftInstancesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ShiftInstances
            .AsNoTracking()
            .Where(s => s.MineSiteId == request.MineSiteId && !s.IsDeleted);

        if (request.FromDate.HasValue)
            query = query.Where(s => s.Date >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(s => s.Date <= request.ToDate.Value);

        var instances = await query
            .OrderByDescending(s => s.Date)
            .ThenBy(s => s.ShiftDefinition.ShiftOrder)
            .Select(s => new ShiftInstanceDto(
                s.Id,
                s.ShiftDefinitionId,
                s.ShiftDefinition.Name,
                s.MineSiteId,
                s.MineSite.Name,
                s.Date.ToString("yyyy-MM-dd"),
                s.SupervisorName,
                s.SupervisorId,
                s.Status,
                s.ActualStartTime,
                s.ActualEndTime,
                s.PersonnelCount,
                s.WeatherConditions,
                s.Notes,
                s.CreatedAt,
                s.Handovers.Count
            ))
            .ToListAsync(cancellationToken);

        return instances;
    }
}
