using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.SafetyIncidents.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.SafetyIncidents.Queries.GetSafetyIncidentById;

public record GetSafetyIncidentByIdQuery(Guid Id) : IRequest<SafetyIncidentDto>;

public class GetSafetyIncidentByIdQueryHandler : IRequestHandler<GetSafetyIncidentByIdQuery, SafetyIncidentDto>
{
    private readonly IApplicationDbContext _context;

    public GetSafetyIncidentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SafetyIncidentDto> Handle(GetSafetyIncidentByIdQuery request, CancellationToken cancellationToken)
    {
        var incident = await _context.SafetyIncidents
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => new SafetyIncidentDto(
                s.Id,
                s.MineSiteId,
                s.MineSite.Name,
                s.MineAreaId,
                s.MineArea != null ? s.MineArea.Name : null,
                s.IncidentNumber,
                s.Title,
                s.IncidentType,
                s.Severity,
                s.IncidentDateTime,
                s.Location,
                s.Description,
                s.ImmediateActions,
                s.ReportedBy,
                s.ReportedAt,
                s.InjuredPersonName,
                s.InjuredPersonRole,
                s.InjuryType,
                s.BodyPartAffected,
                s.LostTimeDays,
                s.IsReportable,
                s.RegulatoryReference,
                s.WitnessNames,
                s.RootCause,
                s.ContributingFactors,
                s.CorrectiveActions,
                s.CorrectiveActionDueDate,
                s.CorrectiveActionCompletedDate,
                s.Status,
                s.CreatedAt,
                s.Investigations.Count
            ))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Safety incident {request.Id} not found.");

        return incident;
    }
}
