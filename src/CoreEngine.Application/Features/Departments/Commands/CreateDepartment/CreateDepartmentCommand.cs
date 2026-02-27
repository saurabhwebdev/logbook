using MediatR;

namespace CoreEngine.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    string? Code,
    Guid? ParentDepartmentId
) : IRequest<Guid>;
