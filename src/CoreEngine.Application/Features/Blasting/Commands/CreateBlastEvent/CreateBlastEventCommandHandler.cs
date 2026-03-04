using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Blasting.Commands.CreateBlastEvent;

public class CreateBlastEventCommandHandler : IRequestHandler<CreateBlastEventCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateBlastEventCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateBlastEventCommand request, CancellationToken cancellationToken)
    {
        // Generate blast number
        var count = await _context.BlastEvents.CountAsync(cancellationToken) + 1;
        var blastNumber = $"BL-{count:D5}";

        var blastEvent = new BlastEvent
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            BlastNumber = blastNumber,
            Title = request.Title,
            BlastType = request.BlastType,
            ScheduledDateTime = request.ScheduledDateTime,
            Location = request.Location,
            DrillingPattern = request.DrillingPattern,
            NumberOfHoles = request.NumberOfHoles,
            TotalExplosivesKg = request.TotalExplosivesKg,
            ExplosiveType = request.ExplosiveType,
            DetonatorType = request.DetonatorType,
            BlastDesignNotes = request.BlastDesignNotes,
            SafetyRadius = request.SafetyRadius,
            SupervisorName = request.SupervisorName,
            LicensedBlasterName = request.LicensedBlasterName,
            Status = "Planned",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlastEvents.Add(blastEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return blastEvent.Id;
    }
}
