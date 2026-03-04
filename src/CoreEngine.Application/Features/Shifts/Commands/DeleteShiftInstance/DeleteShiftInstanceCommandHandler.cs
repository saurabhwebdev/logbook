using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.DeleteShiftInstance;

public class DeleteShiftInstanceCommandHandler : IRequestHandler<DeleteShiftInstanceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteShiftInstanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteShiftInstanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ShiftInstances
            .Include(s => s.Handovers)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(ShiftInstance), request.Id);

        if (entity.Handovers.Any())
            throw new ConflictException("Cannot delete shift instance with existing handovers. Remove all handovers first.");

        entity.IsDeleted = true;
        entity.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
