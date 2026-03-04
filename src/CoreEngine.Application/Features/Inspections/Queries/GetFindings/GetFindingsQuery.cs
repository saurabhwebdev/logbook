using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Inspections.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Queries.GetFindings;

public record GetFindingsQuery(Guid InspectionId) : IRequest<IReadOnlyList<InspectionFindingDto>>;

public class GetFindingsQueryHandler : IRequestHandler<GetFindingsQuery, IReadOnlyList<InspectionFindingDto>>
{
    private readonly IApplicationDbContext _context;
    public GetFindingsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<InspectionFindingDto>> Handle(GetFindingsQuery request, CancellationToken cancellationToken)
    {
        return await _context.InspectionFindings.AsNoTracking()
            .Include(e => e.Inspection)
            .Where(e => e.InspectionId == request.InspectionId)
            .OrderBy(e => e.FindingNumber)
            .Select(e => new InspectionFindingDto(
                e.Id, e.InspectionId, e.Inspection.Title, e.FindingNumber,
                e.Category, e.Severity, e.Description, e.Location,
                e.RecommendedAction, e.AssignedTo, e.ActionDueDate, e.ActionCompletedDate,
                e.Status, e.ClosureNotes, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
