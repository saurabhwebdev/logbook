using CoreEngine.Application.Common.Models;
using MediatR;

namespace CoreEngine.Application.Features.Workflows.Queries.GetMyTasks;

public record GetMyTasksQuery(
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<WorkflowTaskDto>>;

public class WorkflowTaskDto
{
    public Guid Id { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public Guid AssignedToUserId { get; set; }
    public string AssignedToUserName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedByUserName { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public string WorkflowDefinitionName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
}
