using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.AmendRegisterEntry;

public class AmendRegisterEntryCommandHandler : IRequestHandler<AmendRegisterEntryCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public AmendRegisterEntryCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(AmendRegisterEntryCommand request, CancellationToken cancellationToken)
    {
        var originalEntry = await _context.RegisterEntries
            .FirstOrDefaultAsync(e => e.Id == request.OriginalEntryId, cancellationToken);

        if (originalEntry is null)
            throw new NotFoundException(nameof(RegisterEntry), request.OriginalEntryId);

        // Auto-calculate next entry number
        var maxEntryNumber = await _context.RegisterEntries
            .Where(e => e.StatutoryRegisterId == originalEntry.StatutoryRegisterId && !e.IsDeleted)
            .MaxAsync(e => (int?)e.EntryNumber, cancellationToken) ?? 0;

        // Mark original entry as Amended
        originalEntry.Status = "Amended";
        originalEntry.ModifiedAt = DateTime.UtcNow;

        // Create new amendment entry
        var amendmentEntry = new RegisterEntry
        {
            Id = Guid.NewGuid(),
            StatutoryRegisterId = originalEntry.StatutoryRegisterId,
            MineSiteId = originalEntry.MineSiteId,
            EntryNumber = maxEntryNumber + 1,
            EntryDate = request.EntryDate,
            ShiftInstanceId = request.ShiftInstanceId,
            MineAreaId = request.MineAreaId,
            Subject = request.Subject,
            Details = request.Details,
            ReportedBy = request.ReportedBy,
            WitnessName = request.WitnessName,
            ActionTaken = request.ActionTaken,
            ActionDueDate = request.ActionDueDate,
            ActionCompletedDate = request.ActionCompletedDate,
            Status = request.Status ?? "Open",
            AmendmentOfEntryId = request.OriginalEntryId,
            AmendmentReason = request.AmendmentReason,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.RegisterEntries.Add(amendmentEntry);
        await _context.SaveChangesAsync(cancellationToken);

        return amendmentEntry.Id;
    }
}
