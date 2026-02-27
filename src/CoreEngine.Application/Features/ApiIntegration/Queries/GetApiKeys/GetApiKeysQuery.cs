using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.ApiIntegration.Queries.GetApiKeys;

public record ApiKeyDto(Guid Id, string Name, string KeyPrefix, DateTime? ExpiresAt, bool IsActive, DateTime? LastUsedAt, string? Scopes, DateTime CreatedAt);

public record GetApiKeysQuery : IRequest<List<ApiKeyDto>>;

public class GetApiKeysQueryHandler : IRequestHandler<GetApiKeysQuery, List<ApiKeyDto>>
{
    private readonly IApplicationDbContext _context;
    public GetApiKeysQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ApiKeyDto>> Handle(GetApiKeysQuery request, CancellationToken ct)
    {
        return await _context.ApiKeys.OrderByDescending(k => k.CreatedAt)
            .Select(k => new ApiKeyDto(k.Id, k.Name, k.KeyPrefix, k.ExpiresAt, k.IsActive, k.LastUsedAt, k.Scopes, k.CreatedAt))
            .ToListAsync(ct);
    }
}
