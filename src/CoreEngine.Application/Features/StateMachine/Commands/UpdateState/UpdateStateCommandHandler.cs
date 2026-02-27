using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Commands.UpdateState;

public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateStateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _context.StateDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (state is null)
            throw new NotFoundException(nameof(StateDefinition), request.Id);

        // Check for duplicate state name within the same entity type (excluding current record)
        var exists = await _context.StateDefinitions
            .AnyAsync(s => s.EntityType == state.EntityType
                && s.StateName == request.StateName
                && s.Id != request.Id, cancellationToken);

        if (exists)
            throw new ConflictException($"State '{request.StateName}' already exists for entity type '{state.EntityType}'.");

        // If IsInitial is true, ensure no other initial state exists for this entity type
        if (request.IsInitial)
        {
            var hasInitialState = await _context.StateDefinitions
                .AnyAsync(s => s.EntityType == state.EntityType
                    && s.IsInitial
                    && s.Id != request.Id, cancellationToken);

            if (hasInitialState)
                throw new ConflictException($"An initial state already exists for entity type '{state.EntityType}'.");
        }

        // Update fields
        state.StateName = request.StateName;
        state.IsInitial = request.IsInitial;
        state.IsFinal = request.IsFinal;
        state.Color = request.Color;
        state.SortOrder = request.SortOrder;
        state.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
