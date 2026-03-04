using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Blasting.Commands.DeleteBlastEvent;

public record DeleteBlastEventCommand(Guid Id) : IRequest;

public class DeleteBlastEventCommandHandler : IRequestHandler<DeleteBlastEventCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBlastEventCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBlastEventCommand request, CancellationToken cancellationToken)
    {
        var blastEvent = await _context.BlastEvents
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Blast event {request.Id} not found.");

        _context.BlastEvents.Remove(blastEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
