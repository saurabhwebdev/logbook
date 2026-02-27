using System.Text.Json;
using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Commands.StartWorkflow;

public class StartWorkflowCommandHandler : IRequestHandler<StartWorkflowCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeService _dateTimeService;

    public StartWorkflowCommandHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        IDateTimeService dateTimeService)
    {
        _context = context;
        _tenantContext = tenantContext;
        _dateTimeService = dateTimeService;
    }

    public async Task<Guid> Handle(StartWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowDefinition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == request.WorkflowDefinitionId && d.IsActive, cancellationToken);

        if (workflowDefinition == null)
            throw new NotFoundException(nameof(WorkflowDefinition), request.WorkflowDefinitionId);

        // Parse configuration to get first step
        var config = JsonSerializer.Deserialize<WorkflowConfiguration>(workflowDefinition.ConfigurationJson);
        var firstStep = config?.Steps?.FirstOrDefault()?.Name ?? "Start";

        var workflowInstance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowDefinitionId = request.WorkflowDefinitionId,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Status = "Running",
            CurrentStepName = firstStep,
            ContextJson = "{}",
            TenantId = _tenantContext.TenantId,
            CreatedAt = _dateTimeService.UtcNow
        };

        _context.WorkflowInstances.Add(workflowInstance);
        await _context.SaveChangesAsync(cancellationToken);

        return workflowInstance.Id;
    }

    private class WorkflowConfiguration
    {
        public List<WorkflowStep> Steps { get; set; } = new();
    }

    private class WorkflowStep
    {
        public string Name { get; set; } = string.Empty;
    }
}
