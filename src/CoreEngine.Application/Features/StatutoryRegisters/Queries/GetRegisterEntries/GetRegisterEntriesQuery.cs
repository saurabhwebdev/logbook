using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.StatutoryRegisters.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Queries.GetRegisterEntries;

public record GetRegisterEntriesQuery(Guid StatutoryRegisterId, string? Status, DateTime? FromDate, DateTime? ToDate) : IRequest<IReadOnlyList<RegisterEntryDto>>;

public class GetRegisterEntriesQueryHandler : IRequestHandler<GetRegisterEntriesQuery, IReadOnlyList<RegisterEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRegisterEntriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RegisterEntryDto>> Handle(GetRegisterEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.RegisterEntries
            .AsNoTracking()
            .Where(e => e.StatutoryRegisterId == request.StatutoryRegisterId && !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(e => e.Status == request.Status);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(e => e.EntryDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(e => e.EntryDate <= request.ToDate.Value);
        }

        var entries = await query
            .OrderByDescending(e => e.EntryNumber)
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
            .ToListAsync(cancellationToken);

        return entries;
    }
}
