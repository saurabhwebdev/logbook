using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.WorkPermits.Commands.CreateWorkPermit;

public class CreateWorkPermitCommandHandler : IRequestHandler<CreateWorkPermitCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateWorkPermitCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateWorkPermitCommand request, CancellationToken cancellationToken)
    {
        // Generate permit number
        var count = await _context.WorkPermits.CountAsync(cancellationToken) + 1;
        var permitNumber = $"PTW-{count:D5}";

        var workPermit = new WorkPermit
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            PermitNumber = permitNumber,
            Title = request.Title,
            PermitType = request.PermitType,
            RequestedBy = request.RequestedBy,
            RequestDate = request.RequestDate,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            Location = request.Location,
            WorkDescription = request.WorkDescription,
            HazardsIdentified = request.HazardsIdentified,
            ControlMeasures = request.ControlMeasures,
            PPERequired = request.PPERequired,
            EmergencyProcedures = request.EmergencyProcedures,
            GasTestRequired = request.GasTestRequired,
            GasTestResults = request.GasTestResults,
            Notes = request.Notes,
            Status = "Draft",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.WorkPermits.Add(workPermit);
        await _context.SaveChangesAsync(cancellationToken);

        return workPermit.Id;
    }
}
