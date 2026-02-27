using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using CoreEngine.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.API.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantContext = context.RequestServices.GetRequiredService<ITenantContext>();
        var dbContext = context.RequestServices.GetRequiredService<IApplicationDbContext>();

        // Skip tenant resolution for unauthenticated endpoints
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (path.Contains("/auth/login") || path.Contains("/swagger"))
        {
            // For login: try to resolve tenant from header, fall back to default
            if (path.Contains("/auth/login"))
            {
                await TryResolveTenant(context, tenantContext, dbContext);
            }
            await _next(context);
            return;
        }

        // For authenticated requests: resolve from JWT claim first, then header
        var tenantIdClaim = context.User?.FindFirst("tenantId")?.Value;
        if (Guid.TryParse(tenantIdClaim, out var claimTenantId) && claimTenantId != Guid.Empty)
        {
            var tenant = await dbContext.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == claimTenantId && t.IsActive && !t.IsDeleted);

            if (tenant is not null)
            {
                tenantContext.SetTenant(tenant.Id, tenant.Name);
                await _next(context);
                return;
            }
        }

        // Fall back to header-based resolution
        await TryResolveTenant(context, tenantContext, dbContext);
        await _next(context);
    }

    private static async Task TryResolveTenant(
        HttpContext context, ITenantContext tenantContext, IApplicationDbContext dbContext)
    {
        // Strategy 1: X-Tenant-Id header
        if (context.Request.Headers.TryGetValue(AppConstants.TenantHeaderName, out var headerValue)
            && Guid.TryParse(headerValue, out var headerTenantId))
        {
            var tenant = await dbContext.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == headerTenantId && t.IsActive && !t.IsDeleted);

            if (tenant is not null)
            {
                tenantContext.SetTenant(tenant.Id, tenant.Name);
                return;
            }
        }

        // Strategy 2: Subdomain
        var host = context.Request.Host.Host;
        var subdomain = host.Split('.').FirstOrDefault();
        if (!string.IsNullOrEmpty(subdomain) && subdomain != "localhost" && subdomain != "www")
        {
            var tenant = await dbContext.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive && !t.IsDeleted);

            if (tenant is not null)
            {
                tenantContext.SetTenant(tenant.Id, tenant.Name);
                return;
            }
        }

        // Strategy 3: Default tenant (development convenience)
        var defaultTenant = await dbContext.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Subdomain == AppConstants.DefaultTenantSubdomain && !t.IsDeleted);

        if (defaultTenant is not null)
        {
            tenantContext.SetTenant(defaultTenant.Id, defaultTenant.Name);
        }
    }
}
