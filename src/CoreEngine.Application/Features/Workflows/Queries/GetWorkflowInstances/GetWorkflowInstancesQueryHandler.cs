using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Queries.GetWorkflowInstances;

public class GetWorkflowInstancesQueryHandler : IRequestHandler<GetWorkflowInstancesQuery, PaginatedList<WorkflowInstanceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkflowInstancesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<WorkflowInstanceDto>> Handle(GetWorkflowInstancesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .AsQueryable();

        if (request.WorkflowDefinitionId.HasValue)
        {
            query = query.Where(i => i.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.EntityType))
        {
            query = query.Where(i => i.EntityType == request.EntityType);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(i => i.Status == request.Status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new WorkflowInstanceDto
            {
                Id = i.Id,
                WorkflowDefinitionId = i.WorkflowDefinitionId,
                WorkflowDefinitionName = i.WorkflowDefinition.Name,
                EntityType = i.EntityType,
                EntityId = i.EntityId,
                Status = i.Status,
                CurrentStepName = i.CurrentStepName,
                CreatedAt = i.CreatedAt,
                CompletedAt = i.CompletedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<WorkflowInstanceDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
