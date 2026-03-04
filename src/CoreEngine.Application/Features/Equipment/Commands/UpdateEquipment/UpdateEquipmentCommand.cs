using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Commands.UpdateEquipment;

public record UpdateEquipmentCommand(
    Guid Id,
    string Name,
    string Category,
    string? Make,
    string? Model,
    string? SerialNumber,
    int? YearOfManufacture,
    decimal? PurchaseCost,
    string Status,
    string? Location,
    string? OperatorName,
    double? HoursOperated,
    double? NextServiceHours,
    DateTime? NextServiceDate,
    DateTime? LastServiceDate,
    string? WarrantyInfo,
    string? Notes) : IRequest;

public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateEquipmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Equipment
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Equipment {request.Id} not found.");

        entity.Name = request.Name;
        entity.Category = request.Category;
        entity.Make = request.Make;
        entity.Model = request.Model;
        entity.SerialNumber = request.SerialNumber;
        entity.YearOfManufacture = request.YearOfManufacture;
        entity.PurchaseCost = request.PurchaseCost;
        entity.Status = request.Status;
        entity.Location = request.Location;
        entity.OperatorName = request.OperatorName;
        entity.HoursOperated = request.HoursOperated;
        entity.NextServiceHours = request.NextServiceHours;
        entity.NextServiceDate = request.NextServiceDate;
        entity.LastServiceDate = request.LastServiceDate;
        entity.WarrantyInfo = request.WarrantyInfo;
        entity.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
