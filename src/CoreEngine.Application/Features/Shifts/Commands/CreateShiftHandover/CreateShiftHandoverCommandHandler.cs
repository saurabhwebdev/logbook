using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftHandover;

public class CreateShiftHandoverCommandHandler : IRequestHandler<CreateShiftHandoverCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateShiftHandoverCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateShiftHandoverCommand request, CancellationToken cancellationToken)
    {
        var mineSiteExists = await _context.MineSites
            .AnyAsync(m => m.Id == request.MineSiteId, cancellationToken);

        if (!mineSiteExists)
            throw new NotFoundException(nameof(MineSite), request.MineSiteId);

        var outgoingExists = await _context.ShiftInstances
            .AnyAsync(s => s.Id == request.OutgoingShiftInstanceId && s.MineSiteId == request.MineSiteId, cancellationToken);

        if (!outgoingExists)
            throw new NotFoundException(nameof(ShiftInstance), request.OutgoingShiftInstanceId);

        if (request.IncomingShiftInstanceId.HasValue)
        {
            var incomingExists = await _context.ShiftInstances
                .AnyAsync(s => s.Id == request.IncomingShiftInstanceId.Value && s.MineSiteId == request.MineSiteId, cancellationToken);

            if (!incomingExists)
                throw new NotFoundException(nameof(ShiftInstance), request.IncomingShiftInstanceId.Value);
        }

        var entity = new ShiftHandover
        {
            Id = Guid.NewGuid(),
            OutgoingShiftInstanceId = request.OutgoingShiftInstanceId,
            IncomingShiftInstanceId = request.IncomingShiftInstanceId,
            MineSiteId = request.MineSiteId,
            HandoverDateTime = request.HandoverDateTime,
            SafetyIssues = request.SafetyIssues,
            OngoingOperations = request.OngoingOperations,
            PendingTasks = request.PendingTasks,
            EquipmentStatus = request.EquipmentStatus,
            EnvironmentalConditions = request.EnvironmentalConditions,
            GeneralRemarks = request.GeneralRemarks,
            HandedOverBy = request.HandedOverBy,
            ReceivedBy = request.ReceivedBy,
            Status = request.Status ?? "Draft",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ShiftHandovers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
