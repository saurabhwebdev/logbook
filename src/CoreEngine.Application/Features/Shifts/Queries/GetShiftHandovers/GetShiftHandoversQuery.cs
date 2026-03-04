using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Shifts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Queries.GetShiftHandovers;

public record GetShiftHandoversQuery(Guid MineSiteId, DateOnly? FromDate, DateOnly? ToDate) : IRequest<IReadOnlyList<ShiftHandoverDto>>;

public class GetShiftHandoversQueryHandler : IRequestHandler<GetShiftHandoversQuery, IReadOnlyList<ShiftHandoverDto>>
{
    private readonly IApplicationDbContext _context;

    public GetShiftHandoversQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ShiftHandoverDto>> Handle(GetShiftHandoversQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ShiftHandovers
            .AsNoTracking()
            .Where(h => h.MineSiteId == request.MineSiteId && !h.IsDeleted);

        if (request.FromDate.HasValue)
        {
            var fromDateTime = request.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(h => h.HandoverDateTime >= fromDateTime);
        }

        if (request.ToDate.HasValue)
        {
            var toDateTime = request.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(h => h.HandoverDateTime <= toDateTime);
        }

        var handovers = await query
            .OrderByDescending(h => h.HandoverDateTime)
            .Select(h => new ShiftHandoverDto(
                h.Id,
                h.OutgoingShiftInstanceId,
                h.OutgoingShiftInstance.ShiftDefinition.Name + " - " + h.OutgoingShiftInstance.Date.ToString("yyyy-MM-dd"),
                h.IncomingShiftInstanceId,
                h.IncomingShiftInstance != null
                    ? h.IncomingShiftInstance.ShiftDefinition.Name + " - " + h.IncomingShiftInstance.Date.ToString("yyyy-MM-dd")
                    : null,
                h.MineSiteId,
                h.MineSite.Name,
                h.HandoverDateTime,
                h.SafetyIssues,
                h.OngoingOperations,
                h.PendingTasks,
                h.EquipmentStatus,
                h.EnvironmentalConditions,
                h.GeneralRemarks,
                h.HandedOverBy,
                h.ReceivedBy,
                h.Status,
                h.AcknowledgedAt,
                h.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return handovers;
    }
}
