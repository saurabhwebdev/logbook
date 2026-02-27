using MediatR;

namespace CoreEngine.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    List<Guid> PermissionIds
) : IRequest<Unit>;
