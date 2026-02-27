using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.DemoTasks.Commands.TransitionDemoTask;

public record TransitionDemoTaskCommand(Guid Id, string TriggerName, string? Comments) : IRequest;

public class TransitionDemoTaskCommandHandler : IRequestHandler<TransitionDemoTaskCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TransitionDemoTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(TransitionDemoTaskCommand request, CancellationToken ct)
    {
        var task = await _context.DemoTasks.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("DemoTask", request.Id);

        var transition = await _context.StateTransitionDefinitions
            .FirstOrDefaultAsync(t =>
                t.EntityType == "Task" &&
                t.FromState == task.CurrentState &&
                t.TriggerName == request.TriggerName, ct)
            ?? throw new NotFoundException("No valid transition found");

        var fromState = task.CurrentState;
        task.CurrentState = transition.ToState;

        var log = new StateTransitionLog
        {
            EntityType = "Task",
            EntityId = task.Id.ToString(),
            FromState = fromState,
            ToState = transition.ToState,
            TriggerName = request.TriggerName,
            PerformedBy = _currentUser.UserId,
            Comments = request.Comments,
            TransitionedAt = DateTime.UtcNow,
        };
        _context.StateTransitionLogs.Add(log);
        await _context.SaveChangesAsync(ct);
    }
}
