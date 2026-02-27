using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.DemoTasks.Commands.CreateDemoTask;

public record CreateDemoTaskCommand(string Title, string? Description, string? AssignedTo, string Priority) : IRequest<Guid>;

public class CreateDemoTaskCommandHandler : IRequestHandler<CreateDemoTaskCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateDemoTaskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateDemoTaskCommand request, CancellationToken ct)
    {
        var task = new DemoTask
        {
            Title = request.Title,
            Description = request.Description,
            AssignedTo = request.AssignedTo,
            Priority = request.Priority,
            CurrentState = "Draft",
        };
        _context.DemoTasks.Add(task);
        await _context.SaveChangesAsync(ct);
        return task.Id;
    }
}
