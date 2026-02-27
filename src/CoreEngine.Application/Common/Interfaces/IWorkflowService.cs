using CoreEngine.Domain.Entities;

namespace CoreEngine.Application.Common.Interfaces;

public interface IWorkflowService
{
    Task<WorkflowTask> CreateTaskAsync(Guid workflowInstanceId, string taskName, string taskType, Guid assignedToUserId, int priority = 2, DateTime? dueDate = null);
    Task CompleteTaskAsync(Guid taskId, string status, string? comments = null);
    Task<string?> GetNextStepAsync(Guid workflowDefinitionId, string currentStepName);
    Task AdvanceWorkflowAsync(Guid workflowInstanceId);
}
