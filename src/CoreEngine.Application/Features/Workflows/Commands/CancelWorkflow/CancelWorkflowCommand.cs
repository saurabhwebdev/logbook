using MediatR;

namespace CoreEngine.Application.Features.Workflows.Commands.CancelWorkflow;

public record CancelWorkflowCommand(Guid WorkflowInstanceId) : IRequest;
