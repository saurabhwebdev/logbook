using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.CreateFinding;

public record CreateFindingCommand(
    Guid InspectionId,
    string Category,
    string Severity,
    string Description,
    string? Location,
    string? RecommendedAction,
    string? AssignedTo,
    DateTime? ActionDueDate) : IRequest<Guid>;

public class CreateFindingCommandHandler : IRequestHandler<CreateFindingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateFindingCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateFindingCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.InspectionFindings
            .Where(f => f.InspectionId == request.InspectionId)
            .CountAsync(cancellationToken);

        var entity = new InspectionFinding
        {
            InspectionId = request.InspectionId,
            FindingNumber = $"F-{(count + 1):D3}",
            Category = request.Category,
            Severity = request.Severity,
            Description = request.Description,
            Location = request.Location,
            RecommendedAction = request.RecommendedAction,
            AssignedTo = request.AssignedTo,
            ActionDueDate = request.ActionDueDate,
        };

        _context.InspectionFindings.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
