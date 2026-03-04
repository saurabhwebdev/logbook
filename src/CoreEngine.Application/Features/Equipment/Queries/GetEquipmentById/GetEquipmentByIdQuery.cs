using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Equipment.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Queries.GetEquipmentById;

public record GetEquipmentByIdQuery(Guid Id) : IRequest<EquipmentDto>;

public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto>
{
    private readonly IApplicationDbContext _context;
    public GetEquipmentByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<EquipmentDto> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Equipment.AsNoTracking()
            .Include(e => e.MineSite)
            .Include(e => e.MineArea)
            .Where(e => e.Id == request.Id)
            .Select(e => new EquipmentDto(
                e.Id, e.MineSiteId, e.MineSite.Name, e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.AssetNumber, e.Name, e.Category, e.Make, e.Model, e.SerialNumber,
                e.YearOfManufacture, e.PurchaseDate, e.PurchaseCost, e.Status,
                e.Location, e.OperatorName, e.HoursOperated, e.NextServiceHours,
                e.NextServiceDate, e.LastServiceDate, e.WarrantyInfo, e.Notes,
                e.MaintenanceRecords.Count, e.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Equipment {request.Id} not found.");
    }
}
