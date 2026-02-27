using CoreEngine.Domain.Common;
using CoreEngine.Domain.Enums;

namespace CoreEngine.Domain.Entities;

public class User : TenantScopedEntity
{
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; set; }

    // Security
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEndAt { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public Department? Department { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public string FullName => $"{FirstName} {LastName}";
}
