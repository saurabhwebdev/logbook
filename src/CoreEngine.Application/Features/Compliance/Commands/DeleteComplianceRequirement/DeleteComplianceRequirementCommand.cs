using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Compliance.Commands.DeleteComplianceRequirement;

public record DeleteComplianceRequirementCommand(Guid Id) : IRequest;

public class DeleteComplianceRequirementCommandHandler : IRequestHandler<DeleteComplianceRequirementCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteComplianceRequirementCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteComplianceRequirementCommand request, CancellationToken cancellationToken)
    {
        var requirement = await _context.ComplianceRequirements
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Compliance requirement {request.Id} not found.");

        _context.ComplianceRequirements.Remove(requirement);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
