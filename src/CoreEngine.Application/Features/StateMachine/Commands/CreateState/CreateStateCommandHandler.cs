using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Commands.CreateState;

public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateStateCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate state name within the same entity type
        var exists = await _context.StateDefinitions
            .AnyAsync(s => s.EntityType == request.EntityType && s.StateName == request.StateName, cancellationToken);

        if (exists)
            throw new ConflictException($"State '{request.StateName}' already exists for entity type '{request.EntityType}'.");

        // If IsInitial is true, ensure no other initial state exists for this entity type
        if (request.IsInitial)
        {
            var hasInitialState = await _context.StateDefinitions
                .AnyAsync(s => s.EntityType == request.EntityType && s.IsInitial, cancellationToken);

            if (hasInitialState)
                throw new ConflictException($"An initial state already exists for entity type '{request.EntityType}'.");
        }

        var state = new StateDefinition
        {
            Id = Guid.NewGuid(),
            EntityType = request.EntityType,
            StateName = request.StateName,
            IsInitial = request.IsInitial,
            IsFinal = request.IsFinal,
            Color = request.Color,
            SortOrder = request.SortOrder,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StateDefinitions.Add(state);
        await _context.SaveChangesAsync(cancellationToken);

        return state.Id;
    }
}
