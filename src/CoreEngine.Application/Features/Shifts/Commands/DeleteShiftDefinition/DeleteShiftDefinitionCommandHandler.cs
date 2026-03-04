using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.DeleteShiftDefinition;

public class DeleteShiftDefinitionCommandHandler : IRequestHandler<DeleteShiftDefinitionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteShiftDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteShiftDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ShiftDefinitions
            .Include(s => s.ShiftInstances)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(ShiftDefinition), request.Id);

        if (entity.ShiftInstances.Any())
            throw new ConflictException("Cannot delete shift definition with existing shift instances. Remove all instances first.");

        entity.IsDeleted = true;
        entity.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
