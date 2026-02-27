using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Tenants.Commands.UpdateTenantTheme;

public record UpdateTenantThemeCommand(string? LogoUrl, string? PrimaryColor, string? SidebarColor, string? SidebarTextColor) : IRequest;

public class UpdateTenantThemeCommandHandler : IRequestHandler<UpdateTenantThemeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateTenantThemeCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task Handle(UpdateTenantThemeCommand request, CancellationToken ct)
    {
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == _tenantContext.TenantId, ct)
            ?? throw new NotFoundException("Tenant", _tenantContext.TenantId);

        tenant.LogoUrl = request.LogoUrl;
        tenant.PrimaryColor = request.PrimaryColor;
        tenant.SidebarColor = request.SidebarColor;
        tenant.SidebarTextColor = request.SidebarTextColor;
        await _context.SaveChangesAsync(ct);
    }
}
