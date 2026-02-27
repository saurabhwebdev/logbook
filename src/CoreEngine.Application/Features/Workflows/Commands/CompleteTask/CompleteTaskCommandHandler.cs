using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IWorkflowService _workflowService;

    public CompleteTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService,
        IWorkflowService workflowService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
        _workflowService = workflowService;
    }

    public async Task Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(WorkflowTask), request.TaskId);

        if (task.Status != "Pending")
            throw new InvalidOperationException("Task is already completed.");

        task.Status = request.Status;
        task.Comments = request.Comments;
        task.CompletedAt = _dateTimeService.UtcNow;
        task.CompletedByUserId = Guid.Parse(_currentUserService.UserId ?? Guid.Empty.ToString());

        // Advance workflow to next step if task was approved
        if (request.Status == "Approved" || request.Status == "Completed")
        {
            await _workflowService.AdvanceWorkflowAsync(task.WorkflowInstanceId);
        }
        else if (request.Status == "Rejected")
        {
            // Cancel workflow on rejection
            task.WorkflowInstance.Status = "Cancelled";
            task.WorkflowInstance.CompletedAt = _dateTimeService.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
