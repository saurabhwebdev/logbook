using System.Text.Json;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Infrastructure.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeService _dateTimeService;

    public WorkflowService(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        IDateTimeService dateTimeService)
    {
        _context = context;
        _tenantContext = tenantContext;
        _dateTimeService = dateTimeService;
    }

    public async Task<WorkflowTask> CreateTaskAsync(
        Guid workflowInstanceId,
        string taskName,
        string taskType,
        Guid assignedToUserId,
        int priority = 2,
        DateTime? dueDate = null)
    {
        var task = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            WorkflowInstanceId = workflowInstanceId,
            TaskName = taskName,
            TaskType = taskType,
            AssignedToUserId = assignedToUserId,
            Priority = priority,
            DueDate = dueDate,
            Status = "Pending",
            TenantId = _tenantContext.TenantId,
            CreatedAt = _dateTimeService.UtcNow
        };

        _context.WorkflowTasks.Add(task);
        await _context.SaveChangesAsync();

        return task;
    }

    public async Task CompleteTaskAsync(Guid taskId, string status, string? comments = null)
    {
        var task = await _context.WorkflowTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new InvalidOperationException($"Task {taskId} not found");

        task.Status = status;
        task.Comments = comments;
        task.CompletedAt = _dateTimeService.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<string?> GetNextStepAsync(Guid workflowDefinitionId, string currentStepName)
    {
        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == workflowDefinitionId);

        if (definition == null)
            return null;

        // Parse configuration JSON to get workflow steps
        var config = JsonSerializer.Deserialize<WorkflowConfiguration>(definition.ConfigurationJson);
        if (config?.Steps == null || config.Steps.Count == 0)
            return null;

        var currentIndex = config.Steps.FindIndex(s => s.Name == currentStepName);
        if (currentIndex == -1 || currentIndex >= config.Steps.Count - 1)
            return null; // No next step

        return config.Steps[currentIndex + 1].Name;
    }

    public async Task AdvanceWorkflowAsync(Guid workflowInstanceId)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == workflowInstanceId);

        if (instance == null)
            throw new InvalidOperationException($"Workflow instance {workflowInstanceId} not found");

        var nextStep = await GetNextStepAsync(instance.WorkflowDefinitionId, instance.CurrentStepName);

        if (nextStep == null)
        {
            // Workflow completed
            instance.Status = "Completed";
            instance.CompletedAt = _dateTimeService.UtcNow;
        }
        else
        {
            instance.CurrentStepName = nextStep;
        }

        await _context.SaveChangesAsync();
    }

    // Helper class for deserializing workflow configuration
    private class WorkflowConfiguration
    {
        public List<WorkflowStep> Steps { get; set; } = new();
    }

    private class WorkflowStep
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
