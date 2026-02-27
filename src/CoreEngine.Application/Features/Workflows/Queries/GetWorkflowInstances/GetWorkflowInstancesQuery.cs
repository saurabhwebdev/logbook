using CoreEngine.Application.Common.Models;
using MediatR;

namespace CoreEngine.Application.Features.Workflows.Queries.GetWorkflowInstances;

public record GetWorkflowInstancesQuery(
    Guid? WorkflowDefinitionId = null,
    string? EntityType = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<WorkflowInstanceDto>>;

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public string WorkflowDefinitionName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CurrentStepName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
