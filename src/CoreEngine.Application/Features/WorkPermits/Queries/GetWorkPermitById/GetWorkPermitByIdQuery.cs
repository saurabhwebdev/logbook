using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.WorkPermits.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.WorkPermits.Queries.GetWorkPermitById;

public record GetWorkPermitByIdQuery(Guid Id) : IRequest<WorkPermitDto>;

public class GetWorkPermitByIdQueryHandler : IRequestHandler<GetWorkPermitByIdQuery, WorkPermitDto>
{
    private readonly IApplicationDbContext _context;

    public GetWorkPermitByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkPermitDto> Handle(GetWorkPermitByIdQuery request, CancellationToken cancellationToken)
    {
        var workPermit = await _context.WorkPermits
            .AsNoTracking()
            .Where(w => w.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Work permit {request.Id} not found.");

        return workPermit;
    }
}
