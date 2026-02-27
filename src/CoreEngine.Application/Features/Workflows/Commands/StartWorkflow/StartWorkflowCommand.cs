using MediatR;

namespace CoreEngine.Application.Features.Workflows.Commands.StartWorkflow;

public record StartWorkflowCommand(
    Guid WorkflowDefinitionId,
    string EntityType,
    string EntityId
) : IRequest<Guid>;
