using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Commands.UpdateEnvironmentalIncident;

public class UpdateEnvironmentalIncidentCommandHandler : IRequestHandler<UpdateEnvironmentalIncidentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEnvironmentalIncidentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEnvironmentalIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _context.EnvironmentalIncidents
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Environmental incident {request.Id} not found.");

        incident.Title = request.Title;
        incident.IncidentType = request.IncidentType;
        incident.Severity = request.Severity;
        incident.OccurredAt = request.OccurredAt;
        incident.Location = request.Location;
        incident.Description = request.Description;
        incident.ImpactAssessment = request.ImpactAssessment;
        incident.ContainmentActions = request.ContainmentActions;
        incident.RemediationPlan = request.RemediationPlan;
        incident.ReportedBy = request.ReportedBy;
        incident.NotifiedAuthority = request.NotifiedAuthority;
        incident.AuthorityReference = request.AuthorityReference;
        incident.Status = request.Status;
        incident.ClosedAt = request.ClosedAt;
        incident.ClosureNotes = request.ClosureNotes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
