using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.EmailTemplates.Commands.CreateEmailTemplate;

public class CreateEmailTemplateCommandHandler : IRequestHandler<CreateEmailTemplateCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateEmailTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateEmailTemplateCommand request, CancellationToken ct)
    {
        var template = new EmailTemplate
        {
            Name = request.Name,
            Subject = request.Subject,
            HtmlBody = request.HtmlBody,
            PlainTextBody = request.PlainTextBody,
            IsActive = request.IsActive
        };

        _context.EmailTemplates.Add(template);
        await _context.SaveChangesAsync(ct);
        return template.Id;
    }
}
