using MediatR;

namespace CoreEngine.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    LoginUserDto User
);

public record LoginUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    Guid TenantId,
    string TenantName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions
);
