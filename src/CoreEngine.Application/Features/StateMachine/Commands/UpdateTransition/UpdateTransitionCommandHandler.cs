using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Commands.UpdateTransition;

public class UpdateTransitionCommandHandler : IRequestHandler<UpdateTransitionCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTransitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTransitionCommand request, CancellationToken cancellationToken)
    {
        var transition = await _context.StateTransitionDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (transition is null)
            throw new NotFoundException(nameof(StateTransitionDefinition), request.Id);

        // Validate that both states exist for the given entity type
        var fromStateExists = await _context.StateDefinitions
            .AnyAsync(s => s.EntityType == transition.EntityType && s.StateName == request.FromState, cancellationToken);

        if (!fromStateExists)
            throw new NotFoundException($"From state '{request.FromState}' does not exist for entity type '{transition.EntityType}'.");

        var toStateExists = await _context.StateDefinitions
            .AnyAsync(s => s.EntityType == transition.EntityType && s.StateName == request.ToState, cancellationToken);

        if (!toStateExists)
            throw new NotFoundException($"To state '{request.ToState}' does not exist for entity type '{transition.EntityType}'.");

        // Check for duplicate transition (excluding current record)
        var exists = await _context.StateTransitionDefinitions
            .AnyAsync(t => t.EntityType == transition.EntityType
                && t.FromState == request.FromState
                && t.ToState == request.ToState
                && t.TriggerName == request.TriggerName
                && t.Id != request.Id, cancellationToken);

        if (exists)
            throw new ConflictException($"Transition from '{request.FromState}' to '{request.ToState}' with trigger '{request.TriggerName}' already exists.");

        // Update fields
        transition.FromState = request.FromState;
        transition.ToState = request.ToState;
        transition.TriggerName = request.TriggerName;
        transition.RequiredPermission = request.RequiredPermission;
        transition.Description = request.Description;
        transition.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
