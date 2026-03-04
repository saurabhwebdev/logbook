using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftInstance;

public class UpdateShiftInstanceCommandHandler : IRequestHandler<UpdateShiftInstanceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateShiftInstanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateShiftInstanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ShiftInstances
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(ShiftInstance), request.Id);

        entity.SupervisorName = request.SupervisorName;
        entity.SupervisorId = request.SupervisorId;
        entity.Status = request.Status;
        entity.ActualStartTime = request.ActualStartTime;
        entity.ActualEndTime = request.ActualEndTime;
        entity.PersonnelCount = request.PersonnelCount;
        entity.WeatherConditions = request.WeatherConditions;
        entity.Notes = request.Notes;
        entity.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
