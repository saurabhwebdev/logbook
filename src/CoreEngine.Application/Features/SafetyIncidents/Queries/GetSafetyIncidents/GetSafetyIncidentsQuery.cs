using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.SafetyIncidents.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.SafetyIncidents.Queries.GetSafetyIncidents;

public record GetSafetyIncidentsQuery(Guid? MineSiteId = null, string? Status = null, string? Severity = null) : IRequest<IReadOnlyList<SafetyIncidentDto>>;

public class GetSafetyIncidentsQueryHandler : IRequestHandler<GetSafetyIncidentsQuery, IReadOnlyList<SafetyIncidentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSafetyIncidentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SafetyIncidentDto>> Handle(GetSafetyIncidentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SafetyIncidents.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(s => s.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(s => s.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Severity))
            query = query.Where(s => s.Severity == request.Severity);

        return await query
            .OrderByDescending(s => s.IncidentDateTime)
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
            .ToListAsync(cancellationToken);
    }
}
