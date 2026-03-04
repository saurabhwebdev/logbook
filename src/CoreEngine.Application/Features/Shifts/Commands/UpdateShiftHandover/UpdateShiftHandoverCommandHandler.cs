using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftHandover;

public class UpdateShiftHandoverCommandHandler : IRequestHandler<UpdateShiftHandoverCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateShiftHandoverCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateShiftHandoverCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ShiftHandovers
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(ShiftHandover), request.Id);

        if (request.IncomingShiftInstanceId.HasValue)
        {
            var incomingExists = await _context.ShiftInstances
                .AnyAsync(s => s.Id == request.IncomingShiftInstanceId.Value && s.MineSiteId == entity.MineSiteId, cancellationToken);

            if (!incomingExists)
                throw new NotFoundException(nameof(ShiftInstance), request.IncomingShiftInstanceId.Value);
        }

        entity.IncomingShiftInstanceId = request.IncomingShiftInstanceId;
        entity.HandoverDateTime = request.HandoverDateTime;
        entity.SafetyIssues = request.SafetyIssues;
        entity.OngoingOperations = request.OngoingOperations;
        entity.PendingTasks = request.PendingTasks;
        entity.EquipmentStatus = request.EquipmentStatus;
        entity.EnvironmentalConditions = request.EnvironmentalConditions;
        entity.GeneralRemarks = request.GeneralRemarks;
        entity.HandedOverBy = request.HandedOverBy;
        entity.ReceivedBy = request.ReceivedBy;
        entity.Status = request.Status;
        entity.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
