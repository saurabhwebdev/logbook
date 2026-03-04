using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Production.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Queries.GetProductionLogs;

public record GetProductionLogsQuery(
    Guid? MineSiteId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    string? Material = null
) : IRequest<IReadOnlyList<ProductionLogDto>>;

public class GetProductionLogsQueryHandler : IRequestHandler<GetProductionLogsQuery, IReadOnlyList<ProductionLogDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductionLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductionLogDto>> Handle(GetProductionLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ProductionLogs.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(p => p.MineSiteId == request.MineSiteId.Value);

        if (request.DateFrom.HasValue)
            query = query.Where(p => p.Date >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(p => p.Date <= request.DateTo.Value);

        if (!string.IsNullOrEmpty(request.Material))
            query = query.Where(p => p.Material == request.Material);

        return await query
            .OrderByDescending(p => p.Date)
            .ThenByDescending(p => p.CreatedAt)
            .Select(p => new ProductionLogDto(
                p.Id,
                p.MineSiteId,
                p.MineSite.Name,
                p.MineAreaId,
                p.MineArea != null ? p.MineArea.Name : null,
                p.ShiftInstanceId,
                p.LogNumber,
                p.Date,
                p.ShiftName,
                p.Material,
                p.SourceLocation,
                p.DestinationLocation,
                p.QuantityTonnes,
                p.QuantityBCM,
                p.EquipmentUsed,
                p.OperatorName,
                p.HaulingDistance,
                p.LoadCount,
                p.Status,
                p.Notes,
                p.VerifiedBy,
                p.VerifiedAt,
                p.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
