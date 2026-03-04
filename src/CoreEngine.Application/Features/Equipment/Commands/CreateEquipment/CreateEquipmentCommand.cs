using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Commands.CreateEquipment;

public record CreateEquipmentCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
    string Name,
    string Category,
    string? Make,
    string? Model,
    string? SerialNumber,
    int? YearOfManufacture,
    DateTime? PurchaseDate,
    decimal? PurchaseCost,
    string? Location,
    string? OperatorName,
    double? HoursOperated,
    double? NextServiceHours,
    DateTime? NextServiceDate,
    string? WarrantyInfo,
    string? Notes) : IRequest<Guid>;

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateEquipmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.Equipment.CountAsync(cancellationToken);
        var entity = new Domain.Entities.Equipment
        {
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            AssetNumber = $"EQ-{(count + 1):D5}",
            Name = request.Name,
            Category = request.Category,
            Make = request.Make,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            YearOfManufacture = request.YearOfManufacture,
            PurchaseDate = request.PurchaseDate,
            PurchaseCost = request.PurchaseCost,
            Location = request.Location,
            OperatorName = request.OperatorName,
            HoursOperated = request.HoursOperated,
            NextServiceHours = request.NextServiceHours,
            NextServiceDate = request.NextServiceDate,
            WarrantyInfo = request.WarrantyInfo,
            Notes = request.Notes,
        };

        _context.Equipment.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
