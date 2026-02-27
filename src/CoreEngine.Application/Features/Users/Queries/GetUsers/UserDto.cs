namespace CoreEngine.Application.Features.Users.Queries.GetUsers;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Status,
    Guid? DepartmentId,
    string? DepartmentName,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles
);
