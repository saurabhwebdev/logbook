using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Auth.Commands.Login;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate the expired access token to get the user principal
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            throw new ForbiddenAccessException("Invalid access token.");

        var userIdClaim = principal.FindFirst("sub")?.Value
            ?? principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new ForbiddenAccessException("Invalid access token.");

        // Find the user with their refresh tokens
        var user = await _context.Users
            .Include(u => u.Tenant)
            .Include(u => u.RefreshTokens)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User not found.");

        // Find the specific refresh token
        var existingRefreshToken = user.RefreshTokens
            .FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (existingRefreshToken is null || !existingRefreshToken.IsActive)
            throw new ForbiddenAccessException("Invalid or expired refresh token.");

        // Rotate: revoke old, create new
        existingRefreshToken.RevokedAt = DateTime.UtcNow;
        existingRefreshToken.RevokedByIp = _currentUserService.IpAddress;

        var newRefreshToken = _tokenService.GenerateRefreshToken(_currentUserService.IpAddress);
        existingRefreshToken.ReplacedByToken = newRefreshToken.Token;

        newRefreshToken.UserId = user.Id;
        _context.RefreshTokens.Add(newRefreshToken);

        // Collect permissions
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.FullPermission)
            .Distinct()
            .ToList();

        var roleNames = user.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var accessToken = _tokenService.GenerateAccessToken(user, permissions, user.TenantId);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken.Token,
            ExpiresAt: newRefreshToken.ExpiresAt,
            User: new LoginUserDto(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                TenantId: user.TenantId,
                TenantName: user.Tenant.Name,
                Roles: roleNames,
                Permissions: permissions
            )
        );
    }
}
