using MediatR;

namespace CoreEngine.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<Unit>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
