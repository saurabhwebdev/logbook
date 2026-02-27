using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Common.Models;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Queries.GetMyTasks;

public class GetMyTasksQueryHandler : IRequestHandler<GetMyTasksQuery, PaginatedList<WorkflowTaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyTasksQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<WorkflowTaskDto>> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_currentUserService.UserId ?? Guid.Empty.ToString());

        var query = _context.WorkflowTasks
            .Include(t => t.AssignedToUser)
            .Include(t => t.CompletedByUser)
            .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
            .Where(t => t.AssignedToUserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(t => t.Status == request.Status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new WorkflowTaskDto
            {
                Id = t.Id,
                WorkflowInstanceId = t.WorkflowInstanceId,
                TaskName = t.TaskName,
                TaskType = t.TaskType,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUserName = t.AssignedToUser.FullName,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                CompletedByUserName = t.CompletedByUser != null ? t.CompletedByUser.FullName : null,
                Comments = t.Comments,
                CreatedAt = t.CreatedAt,
                WorkflowDefinitionName = t.WorkflowInstance.WorkflowDefinition.Name,
                EntityType = t.WorkflowInstance.EntityType,
                EntityId = t.WorkflowInstance.EntityId
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<WorkflowTaskDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
