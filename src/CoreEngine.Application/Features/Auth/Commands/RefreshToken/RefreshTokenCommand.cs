using MediatR;
using CoreEngine.Application.Features.Auth.Commands.Login;

namespace CoreEngine.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<LoginResponse>;
