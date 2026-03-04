using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.SafetyIncidents.Commands.DeleteSafetyIncident;

public record DeleteSafetyIncidentCommand(Guid Id) : IRequest;

public class DeleteSafetyIncidentCommandHandler : IRequestHandler<DeleteSafetyIncidentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSafetyIncidentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSafetyIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _context.SafetyIncidents
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Safety incident {request.Id} not found.");

        _context.SafetyIncidents.Remove(incident);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
