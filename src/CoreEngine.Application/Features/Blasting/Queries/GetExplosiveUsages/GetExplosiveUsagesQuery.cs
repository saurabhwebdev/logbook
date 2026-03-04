using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Blasting.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Blasting.Queries.GetExplosiveUsages;

public record GetExplosiveUsagesQuery(Guid BlastEventId) : IRequest<IReadOnlyList<ExplosiveUsageDto>>;

public class GetExplosiveUsagesQueryHandler : IRequestHandler<GetExplosiveUsagesQuery, IReadOnlyList<ExplosiveUsageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetExplosiveUsagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ExplosiveUsageDto>> Handle(GetExplosiveUsagesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ExplosiveUsages
            .AsNoTracking()
            .Where(e => e.BlastEventId == request.BlastEventId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new ExplosiveUsageDto(
                e.Id,
                e.BlastEventId,
                e.BlastEvent.Title,
                e.ExplosiveName,
                e.Type,
                e.BatchNumber,
                e.QuantityIssued,
                e.QuantityUsed,
                e.QuantityReturned,
                e.Unit,
                e.MagazineSource,
                e.IssuedBy,
                e.ReceivedBy,
                e.Notes,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
