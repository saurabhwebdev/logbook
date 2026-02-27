using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Configuration.Queries.GetConfigurations;

public record ConfigurationDto(Guid Id, string Key, string Value, string Category, string? Description, string DataType);

public record GetConfigurationsQuery(string? Category = null) : IRequest<List<ConfigurationDto>>;

public class GetConfigurationsQueryHandler : IRequestHandler<GetConfigurationsQuery, List<ConfigurationDto>>
{
    private readonly IApplicationDbContext _context;
    public GetConfigurationsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ConfigurationDto>> Handle(GetConfigurationsQuery request, CancellationToken ct)
    {
        var query = _context.SystemConfigurations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(c => c.Category == request.Category);
        return await query.OrderBy(c => c.Category).ThenBy(c => c.Key)
            .Select(c => new ConfigurationDto(c.Id, c.Key, c.Value, c.Category, c.Description, c.DataType))
            .ToListAsync(ct);
    }
}
