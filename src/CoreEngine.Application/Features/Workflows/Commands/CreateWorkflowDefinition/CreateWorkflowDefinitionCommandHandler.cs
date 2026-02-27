using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Workflows.Commands.CreateWorkflowDefinition;

public class CreateWorkflowDefinitionCommandHandler : IRequestHandler<CreateWorkflowDefinitionCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeService _dateTimeService;

    public CreateWorkflowDefinitionCommandHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        IDateTimeService dateTimeService)
    {
        _context = context;
        _tenantContext = tenantContext;
        _dateTimeService = dateTimeService;
    }

    public async Task<Guid> Handle(CreateWorkflowDefinitionCommand request, CancellationToken cancellationToken)
    {
        var workflowDefinition = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            ConfigurationJson = request.ConfigurationJson,
            IsActive = request.IsActive,
            Version = 1,
            TenantId = _tenantContext.TenantId,
            CreatedAt = _dateTimeService.UtcNow
        };

        _context.WorkflowDefinitions.Add(workflowDefinition);
        await _context.SaveChangesAsync(cancellationToken);

        return workflowDefinition.Id;
    }
}
