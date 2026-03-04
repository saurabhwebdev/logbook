using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Commands.CreateMaintenanceRecord;

public record CreateMaintenanceRecordCommand(
    Guid EquipmentId,
    string MaintenanceType,
    string Priority,
    string Title,
    string? Description,
    DateTime ScheduledDate,
    string? PerformedBy,
    string? Notes) : IRequest<Guid>;

public class CreateMaintenanceRecordCommandHandler : IRequestHandler<CreateMaintenanceRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateMaintenanceRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.MaintenanceRecords.CountAsync(cancellationToken);
        var entity = new MaintenanceRecord
        {
            EquipmentId = request.EquipmentId,
            WorkOrderNumber = $"WO-{(count + 1):D5}",
            MaintenanceType = request.MaintenanceType,
            Priority = request.Priority,
            Title = request.Title,
            Description = request.Description,
            ScheduledDate = request.ScheduledDate,
            PerformedBy = request.PerformedBy,
            Notes = request.Notes,
        };

        _context.MaintenanceRecords.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
