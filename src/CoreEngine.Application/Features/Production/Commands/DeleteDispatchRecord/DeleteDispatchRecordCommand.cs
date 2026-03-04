using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Commands.DeleteDispatchRecord;

public record DeleteDispatchRecordCommand(Guid Id) : IRequest;

public class DeleteDispatchRecordCommandHandler : IRequestHandler<DeleteDispatchRecordCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDispatchRecordCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteDispatchRecordCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DispatchRecords
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Dispatch record {request.Id} not found.");

        _context.DispatchRecords.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
