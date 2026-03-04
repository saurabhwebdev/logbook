using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.StatutoryRegisters.DTOs;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Queries.GetStatutoryRegisterById;

public record GetStatutoryRegisterByIdQuery(Guid Id) : IRequest<StatutoryRegisterDto>;

public class GetStatutoryRegisterByIdQueryHandler : IRequestHandler<GetStatutoryRegisterByIdQuery, StatutoryRegisterDto>
{
    private readonly IApplicationDbContext _context;

    public GetStatutoryRegisterByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StatutoryRegisterDto> Handle(GetStatutoryRegisterByIdQuery request, CancellationToken cancellationToken)
    {
        var register = await _context.StatutoryRegisters
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (register is null)
            throw new NotFoundException(nameof(StatutoryRegister), request.Id);

        return register;
    }
}
