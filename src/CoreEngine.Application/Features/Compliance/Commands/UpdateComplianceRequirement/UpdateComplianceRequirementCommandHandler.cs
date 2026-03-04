using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Compliance.Commands.UpdateComplianceRequirement;

public class UpdateComplianceRequirementCommandHandler : IRequestHandler<UpdateComplianceRequirementCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateComplianceRequirementCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateComplianceRequirementCommand request, CancellationToken cancellationToken)
    {
        var requirement = await _context.ComplianceRequirements
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Compliance requirement {request.Id} not found.");

        requirement.Title = request.Title;
        requirement.Jurisdiction = request.Jurisdiction;
        requirement.Category = request.Category;
        requirement.Description = request.Description;
        requirement.RegulatoryBody = request.RegulatoryBody;
        requirement.ReferenceDocument = request.ReferenceDocument;
        requirement.Frequency = request.Frequency;
        requirement.DueDate = request.DueDate;
        requirement.LastCompletedDate = request.LastCompletedDate;
        requirement.NextDueDate = request.NextDueDate;
        requirement.ResponsibleRole = request.ResponsibleRole;
        requirement.Status = request.Status;
        requirement.Priority = request.Priority;
        requirement.PenaltyForNonCompliance = request.PenaltyForNonCompliance;
        requirement.Notes = request.Notes;
        requirement.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
