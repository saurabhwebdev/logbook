using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Geotechnical.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Queries.GetSurveyRecords;

public record GetSurveyRecordsQuery(Guid? MineSiteId = null, string? SurveyType = null) : IRequest<IReadOnlyList<SurveyRecordDto>>;

public class GetSurveyRecordsQueryHandler : IRequestHandler<GetSurveyRecordsQuery, IReadOnlyList<SurveyRecordDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSurveyRecordsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SurveyRecordDto>> Handle(GetSurveyRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SurveyRecords.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.SurveyType))
            query = query.Where(e => e.SurveyType == request.SurveyType);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new SurveyRecordDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.SurveyNumber,
                e.Title,
                e.SurveyType,
                e.Date,
                e.SurveyorName,
                e.SurveyorLicense,
                e.Location,
                e.Easting,
                e.Northing,
                e.Elevation,
                e.Datum,
                e.CoordinateSystem,
                e.EquipmentUsed,
                e.Accuracy,
                e.VolumeCalculated,
                e.AreaCalculated,
                e.Findings,
                e.Notes,
                e.Status,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
