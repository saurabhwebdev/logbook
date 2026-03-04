using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.UpdateInspection;

public record UpdateInspectionCommand(
    Guid Id,
    string Title,
    DateTime ScheduledDate,
    DateTime? CompletedDate,
    string InspectorName,
    string? InspectorRole,
    string Status,
    string? OverallRating,
    string? Summary,
    string? ChecklistResponsesJson,
    string? WeatherConditions,
    int? PersonnelPresent,
    string? SignedOffBy,
    DateTime? SignedOffAt) : IRequest;

public class UpdateInspectionCommandHandler : IRequestHandler<UpdateInspectionCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateInspectionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateInspectionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Inspections
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Inspection {request.Id} not found.");

        entity.Title = request.Title;
        entity.ScheduledDate = request.ScheduledDate;
        entity.CompletedDate = request.CompletedDate;
        entity.InspectorName = request.InspectorName;
        entity.InspectorRole = request.InspectorRole;
        entity.Status = request.Status;
        entity.OverallRating = request.OverallRating;
        entity.Summary = request.Summary;
        entity.ChecklistResponsesJson = request.ChecklistResponsesJson;
        entity.WeatherConditions = request.WeatherConditions;
        entity.PersonnelPresent = request.PersonnelPresent;
        entity.SignedOffBy = request.SignedOffBy;
        entity.SignedOffAt = request.SignedOffAt;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
