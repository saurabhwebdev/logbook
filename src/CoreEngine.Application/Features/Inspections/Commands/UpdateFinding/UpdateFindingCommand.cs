using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.UpdateFinding;

public record UpdateFindingCommand(
    Guid Id,
    string Category,
    string Severity,
    string Description,
    string? Location,
    string? RecommendedAction,
    string? AssignedTo,
    DateTime? ActionDueDate,
    DateTime? ActionCompletedDate,
    string Status,
    string? ClosureNotes) : IRequest;

public class UpdateFindingCommandHandler : IRequestHandler<UpdateFindingCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateFindingCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateFindingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.InspectionFindings
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"InspectionFinding {request.Id} not found.");

        entity.Category = request.Category;
        entity.Severity = request.Severity;
        entity.Description = request.Description;
        entity.Location = request.Location;
        entity.RecommendedAction = request.RecommendedAction;
        entity.AssignedTo = request.AssignedTo;
        entity.ActionDueDate = request.ActionDueDate;
        entity.ActionCompletedDate = request.ActionCompletedDate;
        entity.Status = request.Status;
        entity.ClosureNotes = request.ClosureNotes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
