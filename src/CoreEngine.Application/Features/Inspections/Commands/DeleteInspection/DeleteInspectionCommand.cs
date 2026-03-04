using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.DeleteInspection;

public record DeleteInspectionCommand(Guid Id) : IRequest;

public class DeleteInspectionCommandHandler : IRequestHandler<DeleteInspectionCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteInspectionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteInspectionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Inspections
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Inspection {request.Id} not found.");

        _context.Inspections.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
