using CoreEngine.Application.Common.Models;
using MediatR;

namespace CoreEngine.Application.Features.Workflows.Queries.GetWorkflowDefinitions;

public record GetWorkflowDefinitionsQuery(
    string? SearchTerm = null,
    string? Category = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<WorkflowDefinitionDto>>;

public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ConfigurationJson { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
}
