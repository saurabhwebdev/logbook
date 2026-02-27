using MediatR;

namespace CoreEngine.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand(
    string Name,
    string? Description,
    List<Guid> PermissionIds
) : IRequest<Guid>;
