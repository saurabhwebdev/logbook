using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Environmental.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Queries.GetEnvironmentalReadings;

public record GetEnvironmentalReadingsQuery(Guid? MineSiteId = null, string? ReadingType = null, string? Status = null) : IRequest<IReadOnlyList<EnvironmentalReadingDto>>;

public class GetEnvironmentalReadingsQueryHandler : IRequestHandler<GetEnvironmentalReadingsQuery, IReadOnlyList<EnvironmentalReadingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEnvironmentalReadingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<EnvironmentalReadingDto>> Handle(GetEnvironmentalReadingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EnvironmentalReadings.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.ReadingType))
            query = query.Where(e => e.ReadingType == request.ReadingType);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EnvironmentalReadingDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.ReadingNumber,
                e.ReadingType,
                e.Parameter,
                e.Value,
                e.Unit,
                e.ThresholdMin,
                e.ThresholdMax,
                e.IsExceedance,
                e.ReadingDateTime,
                e.MonitoringStation,
                e.InstrumentUsed,
                e.CalibratedDate,
                e.RecordedBy,
                e.WeatherConditions,
                e.Notes,
                e.Status,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
