namespace CoreEngine.Shared.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
