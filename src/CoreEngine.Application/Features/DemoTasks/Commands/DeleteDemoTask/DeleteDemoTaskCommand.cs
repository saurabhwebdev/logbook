using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.DemoTasks.Commands.DeleteDemoTask;

public record DeleteDemoTaskCommand(Guid Id) : IRequest;

public class DeleteDemoTaskCommandHandler : IRequestHandler<DeleteDemoTaskCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteDemoTaskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteDemoTaskCommand request, CancellationToken ct)
    {
        var task = await _context.DemoTasks.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("DemoTask", request.Id);
        _context.DemoTasks.Remove(task);
        await _context.SaveChangesAsync(ct);
    }
}
