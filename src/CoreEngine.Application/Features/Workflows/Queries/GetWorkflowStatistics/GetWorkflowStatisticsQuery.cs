using MediatR;

namespace CoreEngine.Application.Features.Workflows.Queries.GetWorkflowStatistics;

public record GetWorkflowStatisticsQuery : IRequest<WorkflowStatisticsDto>;

public class WorkflowStatisticsDto
{
    public int TotalDefinitions { get; set; }
    public int ActiveInstances { get; set; }
    public int CompletedToday { get; set; }
    public int PendingTasks { get; set; }
}
