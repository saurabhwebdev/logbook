using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.EmailTemplates.Commands.DeleteEmailTemplate;

public record DeleteEmailTemplateCommand(Guid Id) : IRequest<Unit>;

public class DeleteEmailTemplateCommandHandler : IRequestHandler<DeleteEmailTemplateCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    public DeleteEmailTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Unit> Handle(DeleteEmailTemplateCommand request, CancellationToken ct)
    {
        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, ct)
            ?? throw new KeyNotFoundException($"Email template {request.Id} not found");

        template.IsDeleted = true;
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
