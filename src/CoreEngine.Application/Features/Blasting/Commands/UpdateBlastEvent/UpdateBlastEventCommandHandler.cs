using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Blasting.Commands.UpdateBlastEvent;

public class UpdateBlastEventCommandHandler : IRequestHandler<UpdateBlastEventCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateBlastEventCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateBlastEventCommand request, CancellationToken cancellationToken)
    {
        var blastEvent = await _context.BlastEvents
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Blast event {request.Id} not found.");

        blastEvent.Title = request.Title;
        blastEvent.BlastType = request.BlastType;
        blastEvent.ScheduledDateTime = request.ScheduledDateTime;
        blastEvent.ActualDateTime = request.ActualDateTime;
        blastEvent.Location = request.Location;
        blastEvent.DrillingPattern = request.DrillingPattern;
        blastEvent.NumberOfHoles = request.NumberOfHoles;
        blastEvent.TotalExplosivesKg = request.TotalExplosivesKg;
        blastEvent.ExplosiveType = request.ExplosiveType;
        blastEvent.DetonatorType = request.DetonatorType;
        blastEvent.Status = request.Status;
        blastEvent.BlastDesignNotes = request.BlastDesignNotes;
        blastEvent.SafetyRadius = request.SafetyRadius;
        blastEvent.EvacuationConfirmed = request.EvacuationConfirmed;
        blastEvent.SentryPostsConfirmed = request.SentryPostsConfirmed;
        blastEvent.PreBlastWarningGiven = request.PreBlastWarningGiven;
        blastEvent.SupervisorName = request.SupervisorName;
        blastEvent.LicensedBlasterName = request.LicensedBlasterName;
        blastEvent.VibrationReading = request.VibrationReading;
        blastEvent.AirBlastReading = request.AirBlastReading;
        blastEvent.PostBlastInspection = request.PostBlastInspection;
        blastEvent.PostBlastNotes = request.PostBlastNotes;
        blastEvent.FragmentationQuality = request.FragmentationQuality;
        blastEvent.MisfireCount = request.MisfireCount;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
