using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.WorkPermits.Commands.DeleteWorkPermit;

public record DeleteWorkPermitCommand(Guid Id) : IRequest;

public class DeleteWorkPermitCommandHandler : IRequestHandler<DeleteWorkPermitCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteWorkPermitCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteWorkPermitCommand request, CancellationToken cancellationToken)
    {
        var workPermit = await _context.WorkPermits
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Work permit {request.Id} not found.");

        _context.WorkPermits.Remove(workPermit);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
