namespace CoreEngine.Application.Features.Search.Queries.GlobalSearch;

public record SearchResultDto(
    string EntityType,
    Guid EntityId,
    string Title,
    string? Description,
    string Url
);

public record GlobalSearchResultDto(
    List<SearchResultDto> Users,
    List<SearchResultDto> Roles,
    List<SearchResultDto> Departments,
    List<SearchResultDto> Files,
    List<SearchResultDto> Reports,
    List<SearchResultDto> DemoTasks,
    List<SearchResultDto> HelpArticles
);
