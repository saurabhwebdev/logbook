using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Commands.DeleteState;

public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteStateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _context.StateDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (state is null)
            throw new NotFoundException(nameof(StateDefinition), request.Id);

        // Check if state is referenced in any transitions
        var isUsedInTransitions = await _context.StateTransitionDefinitions
            .AnyAsync(t => (t.FromState == state.StateName || t.ToState == state.StateName)
                && t.EntityType == state.EntityType, cancellationToken);

        if (isUsedInTransitions)
            throw new ConflictException($"Cannot delete state '{state.StateName}' because it is referenced in one or more transitions.");

        // Check if state is used in transition logs
        var isUsedInLogs = await _context.StateTransitionLogs
            .AnyAsync(l => (l.FromState == state.StateName || l.ToState == state.StateName)
                && l.EntityType == state.EntityType, cancellationToken);

        if (isUsedInLogs)
            throw new ConflictException($"Cannot delete state '{state.StateName}' because it has historical transition records.");

        // Soft delete
        state.IsDeleted = true;
        state.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
