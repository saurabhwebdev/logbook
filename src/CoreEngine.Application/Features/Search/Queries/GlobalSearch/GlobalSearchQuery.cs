using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Search.Queries.GlobalSearch;

public record GlobalSearchQuery(string SearchTerm) : IRequest<GlobalSearchResultDto>;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, GlobalSearchResultDto>
{
    private readonly IApplicationDbContext _context;
    private const int MaxResultsPerEntity = 5;

    public GlobalSearchQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GlobalSearchResultDto> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new GlobalSearchResultDto(
                new List<SearchResultDto>(),
                new List<SearchResultDto>(),
                new List<SearchResultDto>(),
                new List<SearchResultDto>(),
                new List<SearchResultDto>(),
                new List<SearchResultDto>(),
                new List<SearchResultDto>()
            );
        }

        var searchLower = request.SearchTerm.ToLower();

        // Search Users
        var users = await _context.Users
            .Where(u =>
                u.Email.ToLower().Contains(searchLower) ||
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower))
            .OrderBy(u => u.FirstName)
            .Take(MaxResultsPerEntity)
            .Select(u => new SearchResultDto(
                "User",
                u.Id,
                $"{u.FirstName} {u.LastName}",
                u.Email,
                $"/users/{u.Id}"
            ))
            .ToListAsync(cancellationToken);

        // Search Roles
        var roles = await _context.Roles
            .Where(r =>
                r.Name.ToLower().Contains(searchLower) ||
                (r.Description != null && r.Description.ToLower().Contains(searchLower)))
            .OrderBy(r => r.Name)
            .Take(MaxResultsPerEntity)
            .Select(r => new SearchResultDto(
                "Role",
                r.Id,
                r.Name,
                r.Description,
                $"/roles"
            ))
            .ToListAsync(cancellationToken);

        // Search Departments
        var departments = await _context.Departments
            .Where(d =>
                d.Name.ToLower().Contains(searchLower) ||
                (d.Code != null && d.Code.ToLower().Contains(searchLower)))
            .OrderBy(d => d.Name)
            .Take(MaxResultsPerEntity)
            .Select(d => new SearchResultDto(
                "Department",
                d.Id,
                d.Name,
                d.Code,
                $"/departments"
            ))
            .ToListAsync(cancellationToken);

        // Search Files
        var files = await _context.FileMetadata
            .Where(f =>
                f.FileName.ToLower().Contains(searchLower) ||
                f.OriginalFileName.ToLower().Contains(searchLower) ||
                (f.Description != null && f.Description.ToLower().Contains(searchLower)))
            .OrderByDescending(f => f.CreatedAt)
            .Take(MaxResultsPerEntity)
            .Select(f => new SearchResultDto(
                "File",
                f.Id,
                f.OriginalFileName,
                f.Description ?? f.ContentType,
                $"/files"
            ))
            .ToListAsync(cancellationToken);

        // Search Reports
        var reports = await _context.ReportDefinitions
            .Where(r =>
                r.Name.ToLower().Contains(searchLower) ||
                (r.Description != null && r.Description.ToLower().Contains(searchLower)))
            .OrderBy(r => r.Name)
            .Take(MaxResultsPerEntity)
            .Select(r => new SearchResultDto(
                "Report",
                r.Id,
                r.Name,
                r.Description,
                $"/reports"
            ))
            .ToListAsync(cancellationToken);

        // Search DemoTasks
        var demoTasks = await _context.DemoTasks
            .Where(t =>
                t.Title.ToLower().Contains(searchLower) ||
                (t.Description != null && t.Description.ToLower().Contains(searchLower)))
            .OrderByDescending(t => t.CreatedAt)
            .Take(MaxResultsPerEntity)
            .Select(t => new SearchResultDto(
                "DemoTask",
                t.Id,
                t.Title,
                t.Description,
                $"/demo-tasks"
            ))
            .ToListAsync(cancellationToken);

        // Search Help Articles
        var helpArticles = await _context.HelpArticles
            .Where(h =>
                h.IsPublished &&
                (h.Title.ToLower().Contains(searchLower) ||
                h.Content.ToLower().Contains(searchLower) ||
                (h.Tags != null && h.Tags.ToLower().Contains(searchLower))))
            .OrderBy(h => h.Title)
            .Take(MaxResultsPerEntity)
            .Select(h => new SearchResultDto(
                "HelpArticle",
                h.Id,
                h.Title,
                h.Category,
                $"/help/{h.Slug}"
            ))
            .ToListAsync(cancellationToken);

        return new GlobalSearchResultDto(
            users,
            roles,
            departments,
            files,
            reports,
            demoTasks,
            helpArticles
        );
    }
}
