using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Personnel.Commands.DeletePersonnel;

public record DeletePersonnelCommand(Guid Id) : IRequest;

public class DeletePersonnelCommandHandler : IRequestHandler<DeletePersonnelCommand>
{
    private readonly IApplicationDbContext _context;
    public DeletePersonnelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeletePersonnelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Personnel
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Personnel {request.Id} not found.");
        _context.Personnel.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
