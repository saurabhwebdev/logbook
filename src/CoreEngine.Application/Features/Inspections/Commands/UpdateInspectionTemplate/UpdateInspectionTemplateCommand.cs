using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.UpdateInspectionTemplate;

public record UpdateInspectionTemplateCommand(
    Guid Id,
    string Name,
    string Code,
    string Category,
    string? Description,
    string? ChecklistJson,
    string Frequency,
    bool IsActive,
    int SortOrder) : IRequest;

public class UpdateInspectionTemplateCommandHandler : IRequestHandler<UpdateInspectionTemplateCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateInspectionTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateInspectionTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.InspectionTemplates
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"InspectionTemplate {request.Id} not found.");

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Category = request.Category;
        entity.Description = request.Description;
        entity.ChecklistJson = request.ChecklistJson;
        entity.Frequency = request.Frequency;
        entity.IsActive = request.IsActive;
        entity.SortOrder = request.SortOrder;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
