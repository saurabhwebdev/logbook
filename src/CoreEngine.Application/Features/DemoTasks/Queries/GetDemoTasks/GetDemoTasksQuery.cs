using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.DemoTasks.Queries.GetDemoTasks;

public record DemoTaskDto(Guid Id, string Title, string? Description, string CurrentState, string? AssignedTo, string Priority, DateTime CreatedAt);

public record GetDemoTasksQuery : IRequest<List<DemoTaskDto>>;

public class GetDemoTasksQueryHandler : IRequestHandler<GetDemoTasksQuery, List<DemoTaskDto>>
{
    private readonly IApplicationDbContext _context;
    public GetDemoTasksQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<DemoTaskDto>> Handle(GetDemoTasksQuery request, CancellationToken ct)
    {
        return await _context.DemoTasks.OrderByDescending(t => t.CreatedAt)
            .Select(t => new DemoTaskDto(t.Id, t.Title, t.Description, t.CurrentState, t.AssignedTo, t.Priority, t.CreatedAt))
            .ToListAsync(ct);
    }
}
