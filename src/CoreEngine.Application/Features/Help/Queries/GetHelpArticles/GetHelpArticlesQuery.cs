using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Help.Queries.GetHelpArticles;

public record HelpArticleDto(
    Guid Id,
    string Title,
    string Slug,
    string? ModuleKey,
    string Content,
    string? Category,
    int SortOrder,
    bool IsPublished,
    string? Tags,
    DateTime CreatedAt,
    DateTime? ModifiedAt
);

public record GetHelpArticlesQuery(
    string? Category = null,
    string? ModuleKey = null,
    bool PublishedOnly = false
) : IRequest<List<HelpArticleDto>>;

public class GetHelpArticlesQueryHandler : IRequestHandler<GetHelpArticlesQuery, List<HelpArticleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetHelpArticlesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HelpArticleDto>> Handle(GetHelpArticlesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.HelpArticles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(a => a.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.ModuleKey))
            query = query.Where(a => a.ModuleKey == request.ModuleKey);

        if (request.PublishedOnly)
            query = query.Where(a => a.IsPublished);

        var articles = await query
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.Title)
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
            .ToListAsync(cancellationToken);

        return articles;
    }
}
