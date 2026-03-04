using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.WorkPermits.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.WorkPermits.Queries.GetWorkPermits;

public record GetWorkPermitsQuery(Guid? MineSiteId = null, string? Status = null, string? PermitType = null) : IRequest<IReadOnlyList<WorkPermitDto>>;

public class GetWorkPermitsQueryHandler : IRequestHandler<GetWorkPermitsQuery, IReadOnlyList<WorkPermitDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkPermitsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WorkPermitDto>> Handle(GetWorkPermitsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WorkPermits.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(w => w.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(w => w.Status == request.Status);

        if (!string.IsNullOrEmpty(request.PermitType))
            query = query.Where(w => w.PermitType == request.PermitType);

        return await query
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkPermitDto(
                w.Id,
                w.MineSiteId,
                w.MineSite.Name,
                w.MineAreaId,
                w.MineArea != null ? w.MineArea.Name : null,
                w.PermitNumber,
                w.Title,
                w.PermitType,
                w.RequestedBy,
                w.RequestDate,
                w.StartDateTime,
                w.EndDateTime,
                w.Location,
                w.WorkDescription,
                w.HazardsIdentified,
                w.ControlMeasures,
                w.PPERequired,
                w.EmergencyProcedures,
                w.GasTestRequired,
                w.GasTestResults,
                w.Status,
                w.ApprovedBy,
                w.ApprovedAt,
                w.ClosedBy,
                w.ClosedAt,
                w.RejectionReason,
                w.Notes,
                w.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
