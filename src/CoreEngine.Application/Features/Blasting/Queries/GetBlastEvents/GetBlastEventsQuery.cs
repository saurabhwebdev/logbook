using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Blasting.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Blasting.Queries.GetBlastEvents;

public record GetBlastEventsQuery(Guid? MineSiteId = null, string? Status = null) : IRequest<IReadOnlyList<BlastEventDto>>;

public class GetBlastEventsQueryHandler : IRequestHandler<GetBlastEventsQuery, IReadOnlyList<BlastEventDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBlastEventsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BlastEventDto>> Handle(GetBlastEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BlastEvents.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(b => b.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(b => b.Status == request.Status);

        return await query
            .OrderByDescending(b => b.ScheduledDateTime)
            .Select(b => new BlastEventDto(
                b.Id,
                b.MineSiteId,
                b.MineSite.Name,
                b.MineAreaId,
                b.MineArea != null ? b.MineArea.Name : null,
                b.BlastNumber,
                b.Title,
                b.BlastType,
                b.ScheduledDateTime,
                b.ActualDateTime,
                b.Location,
                b.DrillingPattern,
                b.NumberOfHoles,
                b.TotalExplosivesKg,
                b.ExplosiveType,
                b.DetonatorType,
                b.Status,
                b.BlastDesignNotes,
                b.SafetyRadius,
                b.EvacuationConfirmed,
                b.SentryPostsConfirmed,
                b.PreBlastWarningGiven,
                b.SupervisorName,
                b.LicensedBlasterName,
                b.VibrationReading,
                b.AirBlastReading,
                b.PostBlastInspection,
                b.PostBlastNotes,
                b.FragmentationQuality,
                b.MisfireCount,
                b.CreatedAt,
                b.ExplosiveUsages.Count
            ))
            .ToListAsync(cancellationToken);
    }
}
