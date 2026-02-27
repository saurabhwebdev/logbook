using MediatR;

namespace CoreEngine.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Guid? DepartmentId,
    List<Guid> RoleIds
) : IRequest<Guid>;
