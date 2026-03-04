using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.SafetyIncidents.Commands.UpdateSafetyIncident;

public class UpdateSafetyIncidentCommandHandler : IRequestHandler<UpdateSafetyIncidentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSafetyIncidentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSafetyIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _context.SafetyIncidents
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Safety incident {request.Id} not found.");

        incident.Title = request.Title;
        incident.IncidentType = request.IncidentType;
        incident.Severity = request.Severity;
        incident.IncidentDateTime = request.IncidentDateTime;
        incident.Location = request.Location;
        incident.Description = request.Description;
        incident.ImmediateActions = request.ImmediateActions;
        incident.InjuredPersonName = request.InjuredPersonName;
        incident.InjuredPersonRole = request.InjuredPersonRole;
        incident.InjuryType = request.InjuryType;
        incident.BodyPartAffected = request.BodyPartAffected;
        incident.LostTimeDays = request.LostTimeDays;
        incident.IsReportable = request.IsReportable;
        incident.RegulatoryReference = request.RegulatoryReference;
        incident.WitnessNames = request.WitnessNames;
        incident.RootCause = request.RootCause;
        incident.ContributingFactors = request.ContributingFactors;
        incident.CorrectiveActions = request.CorrectiveActions;
        incident.CorrectiveActionDueDate = request.CorrectiveActionDueDate;
        incident.CorrectiveActionCompletedDate = request.CorrectiveActionCompletedDate;
        incident.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
