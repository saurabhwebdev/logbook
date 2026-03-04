using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftInstance;

public class CreateShiftInstanceCommandHandler : IRequestHandler<CreateShiftInstanceCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateShiftInstanceCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateShiftInstanceCommand request, CancellationToken cancellationToken)
    {
        var mineSiteExists = await _context.MineSites
            .AnyAsync(m => m.Id == request.MineSiteId, cancellationToken);

        if (!mineSiteExists)
            throw new NotFoundException(nameof(MineSite), request.MineSiteId);

        var shiftDefinitionExists = await _context.ShiftDefinitions
            .AnyAsync(s => s.Id == request.ShiftDefinitionId && s.MineSiteId == request.MineSiteId, cancellationToken);

        if (!shiftDefinitionExists)
            throw new NotFoundException(nameof(ShiftDefinition), request.ShiftDefinitionId);

        var entity = new ShiftInstance
        {
            Id = Guid.NewGuid(),
            ShiftDefinitionId = request.ShiftDefinitionId,
            MineSiteId = request.MineSiteId,
            Date = DateOnly.Parse(request.Date),
            SupervisorName = request.SupervisorName,
            SupervisorId = request.SupervisorId,
            Status = request.Status ?? "Scheduled",
            ActualStartTime = request.ActualStartTime,
            ActualEndTime = request.ActualEndTime,
            PersonnelCount = request.PersonnelCount,
            WeatherConditions = request.WeatherConditions,
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ShiftInstances.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
