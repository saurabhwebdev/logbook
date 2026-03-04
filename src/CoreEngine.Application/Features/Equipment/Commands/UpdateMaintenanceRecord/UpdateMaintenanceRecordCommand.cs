using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Commands.UpdateMaintenanceRecord;

public record UpdateMaintenanceRecordCommand(
    Guid Id,
    string MaintenanceType,
    string Priority,
    string Title,
    string? Description,
    DateTime ScheduledDate,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? PerformedBy,
    string Status,
    double? DowntimeHours,
    decimal? LaborCost,
    decimal? PartsCost,
    string? PartsUsed,
    string? Findings,
    string? ActionsTaken,
    string? Notes) : IRequest;

public class UpdateMaintenanceRecordCommandHandler : IRequestHandler<UpdateMaintenanceRecordCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateMaintenanceRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MaintenanceRecords
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"MaintenanceRecord {request.Id} not found.");

        entity.MaintenanceType = request.MaintenanceType;
        entity.Priority = request.Priority;
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ScheduledDate = request.ScheduledDate;
        entity.StartedAt = request.StartedAt;
        entity.CompletedAt = request.CompletedAt;
        entity.PerformedBy = request.PerformedBy;
        entity.Status = request.Status;
        entity.DowntimeHours = request.DowntimeHours;
        entity.LaborCost = request.LaborCost;
        entity.PartsCost = request.PartsCost;
        entity.PartsUsed = request.PartsUsed;
        entity.Findings = request.Findings;
        entity.ActionsTaken = request.ActionsTaken;
        entity.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
