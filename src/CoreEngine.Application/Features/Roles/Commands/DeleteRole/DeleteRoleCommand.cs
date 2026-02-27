using MediatR;

namespace CoreEngine.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(Guid Id) : IRequest<Unit>;
