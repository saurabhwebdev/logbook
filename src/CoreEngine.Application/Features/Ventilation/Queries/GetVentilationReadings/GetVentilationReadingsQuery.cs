using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Ventilation.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Ventilation.Queries.GetVentilationReadings;

public record GetVentilationReadingsQuery(Guid? MineSiteId = null, string? Status = null) : IRequest<IReadOnlyList<VentilationReadingDto>>;

public class GetVentilationReadingsQueryHandler : IRequestHandler<GetVentilationReadingsQuery, IReadOnlyList<VentilationReadingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVentilationReadingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VentilationReadingDto>> Handle(GetVentilationReadingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VentilationReadings.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.VentilationStatus == request.Status);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new VentilationReadingDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.ReadingNumber,
                e.LocationDescription,
                e.AirflowVelocity,
                e.AirflowVolume,
                e.Temperature,
                e.Humidity,
                e.BarometricPressure,
                e.ReadingDateTime,
                e.RecordedBy,
                e.InstrumentUsed,
                e.DoorStatus,
                e.FanStatus,
                e.VentilationStatus,
                e.Notes,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
