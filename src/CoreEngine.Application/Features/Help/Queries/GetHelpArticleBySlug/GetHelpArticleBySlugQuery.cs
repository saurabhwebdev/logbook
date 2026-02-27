using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Help.Queries.GetHelpArticles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Help.Queries.GetHelpArticleBySlug;

public record GetHelpArticleBySlugQuery(string Slug) : IRequest<HelpArticleDto?>;

public class GetHelpArticleBySlugQueryHandler : IRequestHandler<GetHelpArticleBySlugQuery, HelpArticleDto?>
{
    private readonly IApplicationDbContext _context;

    public GetHelpArticleBySlugQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HelpArticleDto?> Handle(GetHelpArticleBySlugQuery request, CancellationToken cancellationToken)
    {
        var article = await _context.HelpArticles
            .AsNoTracking()
            .Where(a => a.Slug == request.Slug)
            .Select(a => new HelpArticleDto(
                a.Id,
                a.Title,
                a.Slug,
                a.ModuleKey,
                a.Content,
                a.Category,
                a.SortOrder,
                a.IsPublished,
                a.Tags,
                a.CreatedAt,
                a.ModifiedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return article;
    }
}
