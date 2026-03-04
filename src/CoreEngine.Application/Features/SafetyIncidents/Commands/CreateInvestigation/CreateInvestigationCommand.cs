using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.SafetyIncidents.Commands.CreateInvestigation;

public record CreateInvestigationCommand(
    Guid SafetyIncidentId,
    string InvestigatorName,
    DateTime InvestigationDate,
    string Methodology,
    string Findings,
    string? RootCauseAnalysis,
    string? Recommendations,
    string? PreventiveMeasures,
    string? EvidenceReferences
) : IRequest<Guid>;

public class CreateInvestigationCommandHandler : IRequestHandler<CreateInvestigationCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateInvestigationCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateInvestigationCommand request, CancellationToken cancellationToken)
    {
        var investigation = new IncidentInvestigation
        {
            Id = Guid.NewGuid(),
            SafetyIncidentId = request.SafetyIncidentId,
            InvestigatorName = request.InvestigatorName,
            InvestigationDate = request.InvestigationDate,
            Methodology = request.Methodology,
            Findings = request.Findings,
            RootCauseAnalysis = request.RootCauseAnalysis,
            Recommendations = request.Recommendations,
            PreventiveMeasures = request.PreventiveMeasures,
            EvidenceReferences = request.EvidenceReferences,
            Status = "InProgress",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.IncidentInvestigations.Add(investigation);
        await _context.SaveChangesAsync(cancellationToken);

        return investigation.Id;
    }
}
