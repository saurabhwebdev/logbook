using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateRoleCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Check duplicate name in same tenant
        var exists = await _context.Roles
            .AnyAsync(r => r.Name == request.Name, cancellationToken);

        if (exists)
            throw new ConflictException($"A role with the name '{request.Name}' already exists.");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TenantId = _tenantContext.TenantId,
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow
        };

        // Add permission associations
        foreach (var permissionId in request.PermissionIds)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });
        }

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return role.Id;
    }
}
