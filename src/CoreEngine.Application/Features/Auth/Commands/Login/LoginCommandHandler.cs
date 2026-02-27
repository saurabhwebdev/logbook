using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Enums;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ITenantContext tenantContext,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email within tenant scope (global filter handles TenantId)
        var user = await _context.Users
            .Include(u => u.Tenant)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
            throw new NotFoundException("Invalid email or password.");

        // Edge case: locked or inactive users cannot login
        if (user.Status == UserStatus.Locked)
            throw new ForbiddenAccessException("Your account has been locked. Contact your administrator.");

        if (user.Status == UserStatus.Inactive)
            throw new ForbiddenAccessException("Your account is inactive. Contact your administrator.");

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new NotFoundException("Invalid email or password.");

        // Collect permissions from all roles
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.FullPermission)
            .Distinct()
            .ToList();

        var roleNames = user.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user, permissions, user.TenantId);
        var refreshToken = _tokenService.GenerateRefreshToken(_currentUserService.IpAddress);

        // Save refresh token and update last login
        refreshToken.UserId = user.Id;
        _context.RefreshTokens.Add(refreshToken);
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            ExpiresAt: refreshToken.ExpiresAt,
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
