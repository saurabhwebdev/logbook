using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Commands.CreateSurveyRecord;

public class CreateSurveyRecordCommandHandler : IRequestHandler<CreateSurveyRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateSurveyRecordCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateSurveyRecordCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.SurveyRecords.CountAsync(cancellationToken) + 1;
        var surveyNumber = $"SRV-{count:D5}";

        var record = new SurveyRecord
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            SurveyNumber = surveyNumber,
            Title = request.Title,
            SurveyType = request.SurveyType,
            Date = request.Date,
            SurveyorName = request.SurveyorName,
            SurveyorLicense = request.SurveyorLicense,
            Location = request.Location,
            Easting = request.Easting,
            Northing = request.Northing,
            Elevation = request.Elevation,
            Datum = request.Datum,
            CoordinateSystem = request.CoordinateSystem,
            EquipmentUsed = request.EquipmentUsed,
            Accuracy = request.Accuracy,
            VolumeCalculated = request.VolumeCalculated,
            AreaCalculated = request.AreaCalculated,
            Findings = request.Findings,
            Notes = request.Notes,
            Status = "Draft",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.SurveyRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return record.Id;
    }
}
