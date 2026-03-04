using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.Inspections.Commands.CreateInspectionTemplate;

public record CreateInspectionTemplateCommand(
    string Name,
    string Code,
    string Category,
    string? Description,
    string? ChecklistJson,
    string Frequency,
    int SortOrder) : IRequest<Guid>;

public class CreateInspectionTemplateCommandHandler : IRequestHandler<CreateInspectionTemplateCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateInspectionTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateInspectionTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = new InspectionTemplate
        {
            Name = request.Name,
            Code = request.Code,
            Category = request.Category,
            Description = request.Description,
            ChecklistJson = request.ChecklistJson,
            Frequency = request.Frequency,
            SortOrder = request.SortOrder,
        };

        _context.InspectionTemplates.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
