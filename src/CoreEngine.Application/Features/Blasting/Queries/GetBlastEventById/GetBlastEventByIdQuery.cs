using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Blasting.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Blasting.Queries.GetBlastEventById;

public record GetBlastEventByIdQuery(Guid Id) : IRequest<BlastEventDto>;

public class GetBlastEventByIdQueryHandler : IRequestHandler<GetBlastEventByIdQuery, BlastEventDto>
{
    private readonly IApplicationDbContext _context;

    public GetBlastEventByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BlastEventDto> Handle(GetBlastEventByIdQuery request, CancellationToken cancellationToken)
    {
        var blastEvent = await _context.BlastEvents
            .AsNoTracking()
            .Where(b => b.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Blast event {request.Id} not found.");

        return blastEvent;
    }
}
