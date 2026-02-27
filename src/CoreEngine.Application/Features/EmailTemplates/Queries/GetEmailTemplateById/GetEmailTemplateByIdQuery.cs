using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.EmailTemplates.Queries.GetEmailTemplates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.EmailTemplates.Queries.GetEmailTemplateById;

public record GetEmailTemplateByIdQuery(Guid Id) : IRequest<EmailTemplateDto?>;

public class GetEmailTemplateByIdQueryHandler : IRequestHandler<GetEmailTemplateByIdQuery, EmailTemplateDto?>
{
    private readonly IApplicationDbContext _context;
    public GetEmailTemplateByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<EmailTemplateDto?> Handle(GetEmailTemplateByIdQuery request, CancellationToken ct)
    {
        return await _context.EmailTemplates
            .Where(t => t.Id == request.Id && !t.IsDeleted)
            .Select(t => new EmailTemplateDto(t.Id, t.Name, t.Subject, t.HtmlBody, t.PlainTextBody, t.IsActive, t.CreatedAt))
            .FirstOrDefaultAsync(ct);
    }
}
