using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Compliance.Commands.CreateComplianceRequirement;

public class CreateComplianceRequirementCommandHandler : IRequestHandler<CreateComplianceRequirementCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateComplianceRequirementCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateComplianceRequirementCommand request, CancellationToken cancellationToken)
    {
        var requirement = new ComplianceRequirement
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            Code = request.Code,
            Title = request.Title,
            Jurisdiction = request.Jurisdiction,
            Category = request.Category,
            Description = request.Description,
            RegulatoryBody = request.RegulatoryBody,
            ReferenceDocument = request.ReferenceDocument,
            Frequency = request.Frequency,
            DueDate = request.DueDate,
            NextDueDate = request.NextDueDate,
            ResponsibleRole = request.ResponsibleRole,
            Status = "Pending",
            Priority = request.Priority,
            PenaltyForNonCompliance = request.PenaltyForNonCompliance,
            Notes = request.Notes,
            IsActive = true,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ComplianceRequirements.Add(requirement);
        await _context.SaveChangesAsync(cancellationToken);

        return requirement.Id;
    }
}
