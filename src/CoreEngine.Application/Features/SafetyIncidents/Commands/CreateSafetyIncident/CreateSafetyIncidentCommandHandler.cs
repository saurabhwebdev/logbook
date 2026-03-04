using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.SafetyIncidents.Commands.CreateSafetyIncident;

public class CreateSafetyIncidentCommandHandler : IRequestHandler<CreateSafetyIncidentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateSafetyIncidentCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateSafetyIncidentCommand request, CancellationToken cancellationToken)
    {
        // Generate incident number
        var count = await _context.SafetyIncidents.CountAsync(cancellationToken) + 1;
        var incidentNumber = $"INC-{count:D5}";

        var incident = new SafetyIncident
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            IncidentNumber = incidentNumber,
            Title = request.Title,
            IncidentType = request.IncidentType,
            Severity = request.Severity ?? "Low",
            IncidentDateTime = request.IncidentDateTime,
            Location = request.Location,
            Description = request.Description,
            ImmediateActions = request.ImmediateActions,
            ReportedBy = request.ReportedBy,
            ReportedAt = DateTime.UtcNow,
            InjuredPersonName = request.InjuredPersonName,
            InjuredPersonRole = request.InjuredPersonRole,
            InjuryType = request.InjuryType,
            BodyPartAffected = request.BodyPartAffected,
            LostTimeDays = request.LostTimeDays,
            IsReportable = request.IsReportable ?? false,
            RegulatoryReference = request.RegulatoryReference,
            WitnessNames = request.WitnessNames,
            RootCause = request.RootCause,
            ContributingFactors = request.ContributingFactors,
            CorrectiveActions = request.CorrectiveActions,
            CorrectiveActionDueDate = request.CorrectiveActionDueDate,
            Status = "Open",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.SafetyIncidents.Add(incident);
        await _context.SaveChangesAsync(cancellationToken);

        return incident.Id;
    }
}
