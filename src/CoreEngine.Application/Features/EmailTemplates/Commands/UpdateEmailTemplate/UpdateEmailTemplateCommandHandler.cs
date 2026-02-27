using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate;

public class UpdateEmailTemplateCommandHandler : IRequestHandler<UpdateEmailTemplateCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    public UpdateEmailTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Unit> Handle(UpdateEmailTemplateCommand request, CancellationToken ct)
    {
        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, ct)
            ?? throw new KeyNotFoundException($"Email template {request.Id} not found");

        template.Name = request.Name;
        template.Subject = request.Subject;
        template.HtmlBody = request.HtmlBody;
        template.PlainTextBody = request.PlainTextBody;
        template.IsActive = request.IsActive;

        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
