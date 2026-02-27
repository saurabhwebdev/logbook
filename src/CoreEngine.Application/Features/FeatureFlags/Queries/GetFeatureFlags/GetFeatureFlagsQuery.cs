using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.FeatureFlags.Queries.GetFeatureFlags;

public record FeatureFlagDto(Guid Id, string Name, string? Description, bool IsEnabled);

public record GetFeatureFlagsQuery : IRequest<List<FeatureFlagDto>>;

public class GetFeatureFlagsQueryHandler : IRequestHandler<GetFeatureFlagsQuery, List<FeatureFlagDto>>
{
    private readonly IApplicationDbContext _context;
    public GetFeatureFlagsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<FeatureFlagDto>> Handle(GetFeatureFlagsQuery request, CancellationToken ct)
    {
        return await _context.FeatureFlags.OrderBy(f => f.Name)
            .Select(f => new FeatureFlagDto(f.Id, f.Name, f.Description, f.IsEnabled))
            .ToListAsync(ct);
    }
}
