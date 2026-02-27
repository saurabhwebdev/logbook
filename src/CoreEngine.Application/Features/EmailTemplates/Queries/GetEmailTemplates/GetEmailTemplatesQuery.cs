using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.EmailTemplates.Queries.GetEmailTemplates;

public record EmailTemplateDto(Guid Id, string Name, string Subject, string HtmlBody, string? PlainTextBody, bool IsActive, DateTime CreatedAt);

public record GetEmailTemplatesQuery : IRequest<List<EmailTemplateDto>>;

public class GetEmailTemplatesQueryHandler : IRequestHandler<GetEmailTemplatesQuery, List<EmailTemplateDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEmailTemplatesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<EmailTemplateDto>> Handle(GetEmailTemplatesQuery request, CancellationToken ct)
    {
        return await _context.EmailTemplates
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new EmailTemplateDto(t.Id, t.Name, t.Subject, t.HtmlBody, t.PlainTextBody, t.IsActive, t.CreatedAt))
            .ToListAsync(ct);
    }
}
