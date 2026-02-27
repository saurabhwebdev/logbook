using System.Security.Claims;
using CoreEngine.Domain.Entities;

namespace CoreEngine.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> permissions, Guid tenantId);
    RefreshToken GenerateRefreshToken(string? ipAddress);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
