using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Commands.ReassignTask;

public class ReassignTaskCommandHandler : IRequestHandler<ReassignTaskCommand>
{
    private readonly IApplicationDbContext _context;

    public ReassignTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReassignTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.WorkflowTasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(WorkflowTask), request.TaskId);

        if (task.Status != "Pending")
            throw new InvalidOperationException("Cannot reassign a completed task.");

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.NewAssigneeUserId, cancellationToken);

        if (!userExists)
            throw new NotFoundException(nameof(User), request.NewAssigneeUserId);

        task.AssignedToUserId = request.NewAssigneeUserId;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
