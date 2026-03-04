using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.StatutoryRegisters.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Queries.GetStatutoryRegisters;

public record GetStatutoryRegistersQuery(Guid MineSiteId) : IRequest<IReadOnlyList<StatutoryRegisterDto>>;

public class GetStatutoryRegistersQueryHandler : IRequestHandler<GetStatutoryRegistersQuery, IReadOnlyList<StatutoryRegisterDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStatutoryRegistersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<StatutoryRegisterDto>> Handle(GetStatutoryRegistersQuery request, CancellationToken cancellationToken)
    {
        var registers = await _context.StatutoryRegisters
            .AsNoTracking()
            .Where(s => s.MineSiteId == request.MineSiteId && !s.IsDeleted)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Name)
            .Select(s => new StatutoryRegisterDto(
                s.Id,
                s.MineSiteId,
                s.MineSite.Name,
                s.Name,
                s.Code,
                s.RegisterType,
                s.Description,
                s.Jurisdiction,
                s.IsRequired,
                s.RetentionYears,
                s.IsActive,
                s.SortOrder,
                s.CreatedAt,
                s.Entries.Count
            ))
            .ToListAsync(cancellationToken);

        return registers;
    }
}
