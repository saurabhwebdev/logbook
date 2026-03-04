using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.CreateInspection;

public record CreateInspectionCommand(
    Guid InspectionTemplateId,
    Guid MineSiteId,
    Guid? MineAreaId,
    string Title,
    DateTime ScheduledDate,
    string InspectorName,
    string? InspectorRole,
    string? WeatherConditions,
    int? PersonnelPresent) : IRequest<Guid>;

public class CreateInspectionCommandHandler : IRequestHandler<CreateInspectionCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateInspectionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateInspectionCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.Inspections.CountAsync(cancellationToken);
        var entity = new Inspection
        {
            InspectionTemplateId = request.InspectionTemplateId,
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            InspectionNumber = $"INS-{(count + 1):D5}",
            Title = request.Title,
            ScheduledDate = request.ScheduledDate,
            InspectorName = request.InspectorName,
            InspectorRole = request.InspectorRole,
            WeatherConditions = request.WeatherConditions,
            PersonnelPresent = request.PersonnelPresent,
        };

        _context.Inspections.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
