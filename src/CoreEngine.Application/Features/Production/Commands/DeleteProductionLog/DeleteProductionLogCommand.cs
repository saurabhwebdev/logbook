using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Commands.DeleteProductionLog;

public record DeleteProductionLogCommand(Guid Id) : IRequest;

public class DeleteProductionLogCommandHandler : IRequestHandler<DeleteProductionLogCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductionLogCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteProductionLogCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProductionLogs
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Production log {request.Id} not found.");

        _context.ProductionLogs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
