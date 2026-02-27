using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Tenants.Queries.GetTenantTheme;

public record TenantThemeDto(string TenantName, string? LogoUrl, string? PrimaryColor, string? SidebarColor);

public record GetTenantThemeQuery : IRequest<TenantThemeDto>;

public class GetTenantThemeQueryHandler : IRequestHandler<GetTenantThemeQuery, TenantThemeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetTenantThemeQueryHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<TenantThemeDto> Handle(GetTenantThemeQuery request, CancellationToken ct)
    {
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == _tenantContext.TenantId, ct);

        if (tenant is null)
            return new TenantThemeDto("CoreEngine", null, null, null);

        return new TenantThemeDto(tenant.Name, tenant.LogoUrl, tenant.PrimaryColor, tenant.SidebarColor);
    }
}
