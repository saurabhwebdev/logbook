using MediatR;

namespace CoreEngine.Application.Features.Workflows.Commands.CreateWorkflowDefinition;

public record CreateWorkflowDefinitionCommand(
    string Name,
    string Description,
    string Category,
    string ConfigurationJson,
    bool IsActive
) : IRequest<Guid>;
