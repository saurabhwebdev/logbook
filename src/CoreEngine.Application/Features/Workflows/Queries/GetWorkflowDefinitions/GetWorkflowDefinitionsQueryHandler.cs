using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Queries.GetWorkflowDefinitions;

public class GetWorkflowDefinitionsQueryHandler : IRequestHandler<GetWorkflowDefinitionsQuery, PaginatedList<WorkflowDefinitionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkflowDefinitionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<WorkflowDefinitionDto>> Handle(GetWorkflowDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WorkflowDefinitions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(d => d.Name.Contains(request.SearchTerm) || d.Description.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(d => d.Category == request.Category);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new WorkflowDefinitionDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Category = d.Category,
                ConfigurationJson = d.ConfigurationJson,
                IsActive = d.IsActive,
                Version = d.Version,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<WorkflowDefinitionDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
