using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Shifts.DTOs;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Queries.GetShiftInstanceById;

public record GetShiftInstanceByIdQuery(Guid Id) : IRequest<ShiftInstanceDto>;

public class GetShiftInstanceByIdQueryHandler : IRequestHandler<GetShiftInstanceByIdQuery, ShiftInstanceDto>
{
    private readonly IApplicationDbContext _context;

    public GetShiftInstanceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShiftInstanceDto> Handle(GetShiftInstanceByIdQuery request, CancellationToken cancellationToken)
    {
        var instance = await _context.ShiftInstances
            .AsNoTracking()
            .Where(s => s.Id == request.Id && !s.IsDeleted)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (instance is null)
            throw new NotFoundException(nameof(ShiftInstance), request.Id);

        return instance;
    }
}
