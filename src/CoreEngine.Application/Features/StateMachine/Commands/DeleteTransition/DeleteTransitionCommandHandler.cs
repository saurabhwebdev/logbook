using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StateMachine.Commands.DeleteTransition;

public class DeleteTransitionCommandHandler : IRequestHandler<DeleteTransitionCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTransitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTransitionCommand request, CancellationToken cancellationToken)
    {
        var transition = await _context.StateTransitionDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (transition is null)
            throw new NotFoundException(nameof(StateTransitionDefinition), request.Id);

        // Soft delete
        transition.IsDeleted = true;
        transition.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
