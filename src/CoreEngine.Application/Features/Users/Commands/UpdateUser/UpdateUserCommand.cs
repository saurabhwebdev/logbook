using CoreEngine.Domain.Enums;
using MediatR;

namespace CoreEngine.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Guid? DepartmentId,
    UserStatus Status,
    List<Guid> RoleIds
) : IRequest<Unit>;
