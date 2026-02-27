using CoreEngine.Application.Features.Workflows.Queries.GetMyTasks;
using MediatR;

namespace CoreEngine.Application.Features.Workflows.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<WorkflowTaskDto>;
