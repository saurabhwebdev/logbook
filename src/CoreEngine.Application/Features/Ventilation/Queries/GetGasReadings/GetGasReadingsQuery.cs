using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Ventilation.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Ventilation.Queries.GetGasReadings;

public record GetGasReadingsQuery(Guid? MineSiteId = null, string? GasType = null, string? Status = null) : IRequest<IReadOnlyList<GasReadingDto>>;

public class GetGasReadingsQueryHandler : IRequestHandler<GetGasReadingsQuery, IReadOnlyList<GasReadingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetGasReadingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GasReadingDto>> Handle(GetGasReadingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GasReadings.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.GasType))
            query = query.Where(e => e.GasType == request.GasType);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new GasReadingDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.ReadingNumber,
                e.GasType,
                e.Concentration,
                e.Unit,
                e.ThresholdTWA,
                e.ThresholdSTEL,
                e.ThresholdCeiling,
                e.IsExceedance,
                e.LocationDescription,
                e.ReadingDateTime,
                e.RecordedBy,
                e.InstrumentId,
                e.CalibrationDate,
                e.ActionTaken,
                e.Status,
                e.Notes,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
