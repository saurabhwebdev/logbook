using MediatR;

namespace CoreEngine.Application.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    string? Code,
    Guid? ParentDepartmentId
) : IRequest<Unit>;
