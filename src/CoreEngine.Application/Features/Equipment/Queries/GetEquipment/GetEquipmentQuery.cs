using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Equipment.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Queries.GetEquipment;

public record GetEquipmentQuery(Guid? MineSiteId, string? Category, string? Status) : IRequest<IReadOnlyList<EquipmentDto>>;

public class GetEquipmentQueryHandler : IRequestHandler<GetEquipmentQuery, IReadOnlyList<EquipmentDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEquipmentQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<EquipmentDto>> Handle(GetEquipmentQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Equipment.AsNoTracking()
            .Include(e => e.MineSite)
            .Include(e => e.MineArea)
            .AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);
        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(e => e.Category == request.Category);
        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query.OrderBy(e => e.Name)
            .Select(e => new EquipmentDto(
                e.Id, e.MineSiteId, e.MineSite.Name, e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.AssetNumber, e.Name, e.Category, e.Make, e.Model, e.SerialNumber,
                e.YearOfManufacture, e.PurchaseDate, e.PurchaseCost, e.Status,
                e.Location, e.OperatorName, e.HoursOperated, e.NextServiceHours,
                e.NextServiceDate, e.LastServiceDate, e.WarrantyInfo, e.Notes,
                e.MaintenanceRecords.Count, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
