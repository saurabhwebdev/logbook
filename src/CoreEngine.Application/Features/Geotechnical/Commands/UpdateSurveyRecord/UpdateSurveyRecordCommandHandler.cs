using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Commands.UpdateSurveyRecord;

public class UpdateSurveyRecordCommandHandler : IRequestHandler<UpdateSurveyRecordCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSurveyRecordCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSurveyRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.SurveyRecords
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Survey record {request.Id} not found.");

        record.Title = request.Title;
        record.SurveyType = request.SurveyType;
        record.Date = request.Date;
        record.SurveyorName = request.SurveyorName;
        record.SurveyorLicense = request.SurveyorLicense;
        record.Location = request.Location;
        record.Easting = request.Easting;
        record.Northing = request.Northing;
        record.Elevation = request.Elevation;
        record.Datum = request.Datum;
        record.CoordinateSystem = request.CoordinateSystem;
        record.EquipmentUsed = request.EquipmentUsed;
        record.Accuracy = request.Accuracy;
        record.VolumeCalculated = request.VolumeCalculated;
        record.AreaCalculated = request.AreaCalculated;
        record.Findings = request.Findings;
        record.Notes = request.Notes;
        record.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
