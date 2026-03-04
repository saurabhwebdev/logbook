using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Commands.CreateEnvironmentalIncident;

public class CreateEnvironmentalIncidentCommandHandler : IRequestHandler<CreateEnvironmentalIncidentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateEnvironmentalIncidentCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateEnvironmentalIncidentCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.EnvironmentalIncidents.CountAsync(cancellationToken) + 1;
        var incidentNumber = $"EI-{count:D5}";

        var incident = new EnvironmentalIncident
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            IncidentNumber = incidentNumber,
            Title = request.Title,
            IncidentType = request.IncidentType,
            Severity = request.Severity,
            OccurredAt = request.OccurredAt,
            Location = request.Location,
            Description = request.Description,
            ImpactAssessment = request.ImpactAssessment,
            ContainmentActions = request.ContainmentActions,
            RemediationPlan = request.RemediationPlan,
            ReportedBy = request.ReportedBy,
            NotifiedAuthority = request.NotifiedAuthority,
            AuthorityReference = request.AuthorityReference,
            Status = "Open",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.EnvironmentalIncidents.Add(incident);
        await _context.SaveChangesAsync(cancellationToken);

        return incident.Id;
    }
}
