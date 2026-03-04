using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.StatutoryRegisters.DTOs;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Queries.GetRegisterEntryById;

public record GetRegisterEntryByIdQuery(Guid Id) : IRequest<RegisterEntryDto>;

public class GetRegisterEntryByIdQueryHandler : IRequestHandler<GetRegisterEntryByIdQuery, RegisterEntryDto>
{
    private readonly IApplicationDbContext _context;

    public GetRegisterEntryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterEntryDto> Handle(GetRegisterEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var entry = await _context.RegisterEntries
            .AsNoTracking()
            .Where(e => e.Id == request.Id)
            .Select(e => new RegisterEntryDto(
                e.Id,
                e.StatutoryRegisterId,
                e.StatutoryRegister.Name,
                e.MineSiteId,
                e.MineSite.Name,
                e.EntryNumber,
                e.EntryDate,
                e.ShiftInstanceId,
                e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.Subject,
                e.Details,
                e.ReportedBy,
                e.WitnessName,
                e.ActionTaken,
                e.ActionDueDate,
                e.ActionCompletedDate,
                e.Status,
                e.AmendmentOfEntryId,
                e.AmendmentReason,
                e.CreatedAt,
                e.Amendments.Count
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (entry is null)
            throw new NotFoundException(nameof(RegisterEntry), request.Id);

        return entry;
    }
}
