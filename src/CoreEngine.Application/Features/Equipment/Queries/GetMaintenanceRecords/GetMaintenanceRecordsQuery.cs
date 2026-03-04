using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Equipment.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Queries.GetMaintenanceRecords;

public record GetMaintenanceRecordsQuery(Guid? EquipmentId, string? Status) : IRequest<IReadOnlyList<MaintenanceRecordDto>>;

public class GetMaintenanceRecordsQueryHandler : IRequestHandler<GetMaintenanceRecordsQuery, IReadOnlyList<MaintenanceRecordDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMaintenanceRecordsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<MaintenanceRecordDto>> Handle(GetMaintenanceRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MaintenanceRecords.AsNoTracking()
            .Include(e => e.Equipment)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
            query = query.Where(e => e.EquipmentId == request.EquipmentId.Value);
        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query.OrderByDescending(e => e.ScheduledDate)
            .Select(e => new MaintenanceRecordDto(
                e.Id, e.EquipmentId, e.Equipment.Name, e.Equipment.AssetNumber,
                e.WorkOrderNumber, e.MaintenanceType, e.Priority, e.Title,
                e.Description, e.ScheduledDate, e.StartedAt, e.CompletedAt,
                e.PerformedBy, e.Status, e.DowntimeHours, e.LaborCost, e.PartsCost,
                e.PartsUsed, e.Findings, e.ActionsTaken, e.Notes, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
