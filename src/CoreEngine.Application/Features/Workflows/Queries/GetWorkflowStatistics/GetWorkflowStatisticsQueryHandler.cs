using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Queries.GetWorkflowStatistics;

public class GetWorkflowStatisticsQueryHandler : IRequestHandler<GetWorkflowStatisticsQuery, WorkflowStatisticsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeService _dateTimeService;

    public GetWorkflowStatisticsQueryHandler(
        IApplicationDbContext context,
        IDateTimeService dateTimeService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
    }

    public async Task<WorkflowStatisticsDto> Handle(GetWorkflowStatisticsQuery request, CancellationToken cancellationToken)
    {
        var today = _dateTimeService.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var totalDefinitions = await _context.WorkflowDefinitions.CountAsync(cancellationToken);
        var activeInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == "Running", cancellationToken);
        var completedToday = await _context.WorkflowInstances
            .CountAsync(i => i.Status == "Completed" && i.CompletedAt >= today && i.CompletedAt < tomorrow, cancellationToken);
        var pendingTasks = await _context.WorkflowTasks.CountAsync(t => t.Status == "Pending", cancellationToken);

        return new WorkflowStatisticsDto
        {
            TotalDefinitions = totalDefinitions,
            ActiveInstances = activeInstances,
            CompletedToday = completedToday,
            PendingTasks = pendingTasks
        };
    }
}
