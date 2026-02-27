namespace CoreEngine.Application.Features.Departments.Queries.GetDepartments;

public record DepartmentDto(
    Guid Id,
    string Name,
    string? Code,
    Guid? ParentDepartmentId,
    string? ParentDepartmentName,
    DateTime CreatedAt,
    int UserCount
);
