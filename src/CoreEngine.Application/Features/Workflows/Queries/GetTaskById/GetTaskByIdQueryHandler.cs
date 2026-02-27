using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Workflows.Queries.GetMyTasks;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, WorkflowTaskDto>
{
    private readonly IApplicationDbContext _context;

    public GetTaskByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkflowTaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.AssignedToUser)
            .Include(t => t.CompletedByUser)
            .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
            .Where(t => t.Id == request.TaskId)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(WorkflowTask), request.TaskId);

        return task;
    }
}
