using MediatR;

namespace CoreEngine.Application.Features.Workflows.Commands.ReassignTask;

public record ReassignTaskCommand(
    Guid TaskId,
    Guid NewAssigneeUserId
) : IRequest;
