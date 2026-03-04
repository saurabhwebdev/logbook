using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateRegisterEntry;

public class CreateRegisterEntryCommandHandler : IRequestHandler<CreateRegisterEntryCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateRegisterEntryCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateRegisterEntryCommand request, CancellationToken cancellationToken)
    {
        var register = await _context.StatutoryRegisters
            .FirstOrDefaultAsync(s => s.Id == request.StatutoryRegisterId, cancellationToken);

        if (register is null)
            throw new NotFoundException(nameof(StatutoryRegister), request.StatutoryRegisterId);

        // Auto-calculate next entry number
        var maxEntryNumber = await _context.RegisterEntries
            .Where(e => e.StatutoryRegisterId == request.StatutoryRegisterId && !e.IsDeleted)
            .MaxAsync(e => (int?)e.EntryNumber, cancellationToken) ?? 0;

        var entry = new RegisterEntry
        {
            Id = Guid.NewGuid(),
            StatutoryRegisterId = request.StatutoryRegisterId,
            MineSiteId = request.MineSiteId,
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
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.RegisterEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}
