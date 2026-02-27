using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Common.Models;
using CoreEngine.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    Guid? DepartmentId = null,
    UserStatus? Status = null
) : IRequest<PaginatedList<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(search) ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(u => u.DepartmentId == request.DepartmentId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(u => u.Status == request.Status.Value);
        }

        var projectedQuery = query
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.Status.ToString(),
                u.DepartmentId,
                u.Department != null ? u.Department.Name : null,
                u.CreatedAt,
                u.LastLoginAt,
                u.UserRoles.Select(ur => ur.Role.Name).ToList()
            ));

        return await PaginatedList<UserDto>.CreateAsync(projectedQuery, request.Page, request.PageSize);
    }
}
