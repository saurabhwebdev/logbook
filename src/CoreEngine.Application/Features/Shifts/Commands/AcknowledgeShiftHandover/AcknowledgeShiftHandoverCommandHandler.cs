using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.AcknowledgeShiftHandover;

public class AcknowledgeShiftHandoverCommandHandler : IRequestHandler<AcknowledgeShiftHandoverCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public AcknowledgeShiftHandoverCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AcknowledgeShiftHandoverCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ShiftHandovers
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(ShiftHandover), request.Id);

        if (entity.Status == "Acknowledged")
            throw new ConflictException("This handover has already been acknowledged.");

        entity.Status = "Acknowledged";
        entity.AcknowledgedAt = DateTime.UtcNow;
        entity.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
