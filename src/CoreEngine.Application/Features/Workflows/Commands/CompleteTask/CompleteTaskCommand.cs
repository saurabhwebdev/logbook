using MediatR;

namespace CoreEngine.Application.Features.Workflows.Commands.CompleteTask;

public record CompleteTaskCommand(
    Guid TaskId,
    string Status,
    string? Comments = null
) : IRequest;
