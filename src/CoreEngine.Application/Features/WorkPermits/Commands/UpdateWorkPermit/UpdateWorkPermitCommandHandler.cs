using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.WorkPermits.Commands.UpdateWorkPermit;

public class UpdateWorkPermitCommandHandler : IRequestHandler<UpdateWorkPermitCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateWorkPermitCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateWorkPermitCommand request, CancellationToken cancellationToken)
    {
        var workPermit = await _context.WorkPermits
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Work permit {request.Id} not found.");

        workPermit.Title = request.Title;
        workPermit.PermitType = request.PermitType;
        workPermit.RequestedBy = request.RequestedBy;
        workPermit.RequestDate = request.RequestDate;
        workPermit.StartDateTime = request.StartDateTime;
        workPermit.EndDateTime = request.EndDateTime;
        workPermit.Location = request.Location;
        workPermit.WorkDescription = request.WorkDescription;
        workPermit.HazardsIdentified = request.HazardsIdentified;
        workPermit.ControlMeasures = request.ControlMeasures;
        workPermit.PPERequired = request.PPERequired;
        workPermit.EmergencyProcedures = request.EmergencyProcedures;
        workPermit.GasTestRequired = request.GasTestRequired;
        workPermit.GasTestResults = request.GasTestResults;
        workPermit.Status = request.Status;
        workPermit.ApprovedBy = request.ApprovedBy;
        workPermit.ApprovedAt = request.ApprovedAt;
        workPermit.ClosedBy = request.ClosedBy;
        workPermit.ClosedAt = request.ClosedAt;
        workPermit.RejectionReason = request.RejectionReason;
        workPermit.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
