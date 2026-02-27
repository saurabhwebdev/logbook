using CoreEngine.API.Filters;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.API.Controllers;

public class TenantsController : BaseApiController
{
    private readonly IApplicationDbContext _context;

    public TenantsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [RequirePermission(Permissions.Tenants.Read)]
    public async Task<ActionResult<IReadOnlyList<TenantResponse>>> GetAll()
    {
        var tenants = await _context.Tenants
            .IgnoreQueryFilters()
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TenantResponse(
                t.Id,
                t.Name,
                t.Subdomain,
                t.IsActive,
                t.CreatedAt))
            .ToListAsync();

        return Ok(tenants);
    }

    [HttpPost]
    [RequirePermission(Permissions.Tenants.Create)]
    public async Task<ActionResult<Guid>> Create(CreateTenantRequest request)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Subdomain = request.Subdomain,
            IsActive = true
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = tenant.Id }, tenant.Id);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.Tenants.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateTenantRequest request)
    {
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tenant is null) return NotFound();

        tenant.Name = request.Name;
        tenant.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record TenantResponse(Guid Id, string Name, string Subdomain, bool IsActive, DateTime CreatedAt);
public record CreateTenantRequest(string Name, string Subdomain);
public record UpdateTenantRequest(string Name, bool IsActive);
