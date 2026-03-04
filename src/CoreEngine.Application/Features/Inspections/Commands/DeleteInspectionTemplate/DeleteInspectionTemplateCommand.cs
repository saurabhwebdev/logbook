using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Inspections.Commands.DeleteInspectionTemplate;

public record DeleteInspectionTemplateCommand(Guid Id) : IRequest;

public class DeleteInspectionTemplateCommandHandler : IRequestHandler<DeleteInspectionTemplateCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteInspectionTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteInspectionTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.InspectionTemplates
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"InspectionTemplate {request.Id} not found.");

        _context.InspectionTemplates.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
