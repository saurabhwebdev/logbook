using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Production.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Queries.GetDispatchRecords;

public record GetDispatchRecordsQuery(
    Guid? MineSiteId = null,
    string? Status = null
) : IRequest<IReadOnlyList<DispatchRecordDto>>;

public class GetDispatchRecordsQueryHandler : IRequestHandler<GetDispatchRecordsQuery, IReadOnlyList<DispatchRecordDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDispatchRecordsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DispatchRecordDto>> Handle(GetDispatchRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DispatchRecords.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(d => d.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(d => d.Status == request.Status);

        return await query
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new DispatchRecordDto(
                d.Id,
                d.MineSiteId,
                d.MineSite.Name,
                d.DispatchNumber,
                d.Date,
                d.VehicleNumber,
                d.DriverName,
                d.Material,
                d.SourceLocation,
                d.DestinationLocation,
                d.WeighbridgeTicketNumber,
                d.GrossWeight,
                d.TareWeight,
                d.NetWeight,
                d.Unit,
                d.DepartureTime,
                d.ArrivalTime,
                d.Status,
                d.Notes,
                d.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
