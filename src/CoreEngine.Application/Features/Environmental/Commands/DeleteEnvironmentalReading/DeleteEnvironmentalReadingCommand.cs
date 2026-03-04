using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Commands.DeleteEnvironmentalReading;

public record DeleteEnvironmentalReadingCommand(Guid Id) : IRequest;

public class DeleteEnvironmentalReadingCommandHandler : IRequestHandler<DeleteEnvironmentalReadingCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEnvironmentalReadingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteEnvironmentalReadingCommand request, CancellationToken cancellationToken)
    {
        var reading = await _context.EnvironmentalReadings
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Environmental reading {request.Id} not found.");

        _context.EnvironmentalReadings.Remove(reading);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
