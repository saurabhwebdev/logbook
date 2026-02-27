using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Commands.CreateTransition;

public class CreateTransitionCommandHandler : IRequestHandler<CreateTransitionCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateTransitionCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateTransitionCommand request, CancellationToken cancellationToken)
    {
        // Validate that both states exist for the given entity type
        var fromStateExists = await _context.StateDefinitions
            .AnyAsync(s => s.EntityType == request.EntityType && s.StateName == request.FromState, cancellationToken);

        if (!fromStateExists)
            throw new NotFoundException($"From state '{request.FromState}' does not exist for entity type '{request.EntityType}'.");

        var toStateExists = await _context.StateDefinitions
            .AnyAsync(s => s.EntityType == request.EntityType && s.StateName == request.ToState, cancellationToken);

        if (!toStateExists)
            throw new NotFoundException($"To state '{request.ToState}' does not exist for entity type '{request.EntityType}'.");

        // Check for duplicate transition
        var exists = await _context.StateTransitionDefinitions
            .AnyAsync(t => t.EntityType == request.EntityType
                && t.FromState == request.FromState
                && t.ToState == request.ToState
                && t.TriggerName == request.TriggerName, cancellationToken);

        if (exists)
            throw new ConflictException($"Transition from '{request.FromState}' to '{request.ToState}' with trigger '{request.TriggerName}' already exists.");

        var transition = new StateTransitionDefinition
        {
            Id = Guid.NewGuid(),
            EntityType = request.EntityType,
            FromState = request.FromState,
            ToState = request.ToState,
            TriggerName = request.TriggerName,
            RequiredPermission = request.RequiredPermission,
            Description = request.Description,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StateTransitionDefinitions.Add(transition);
        await _context.SaveChangesAsync(cancellationToken);

        return transition.Id;
    }
}
