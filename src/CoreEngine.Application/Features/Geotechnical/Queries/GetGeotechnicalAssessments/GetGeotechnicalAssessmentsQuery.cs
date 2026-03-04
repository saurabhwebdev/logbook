using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.Geotechnical.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Queries.GetGeotechnicalAssessments;

public record GetGeotechnicalAssessmentsQuery(Guid? MineSiteId = null, string? Status = null) : IRequest<IReadOnlyList<GeotechnicalAssessmentDto>>;

public class GetGeotechnicalAssessmentsQueryHandler : IRequestHandler<GetGeotechnicalAssessmentsQuery, IReadOnlyList<GeotechnicalAssessmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetGeotechnicalAssessmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GeotechnicalAssessmentDto>> Handle(GetGeotechnicalAssessmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GeotechnicalAssessments.AsNoTracking().AsQueryable();

        if (request.MineSiteId.HasValue)
            query = query.Where(e => e.MineSiteId == request.MineSiteId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(e => e.Status == request.Status);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new GeotechnicalAssessmentDto(
                e.Id,
                e.MineSiteId,
                e.MineSite.Name,
                e.MineAreaId,
                e.MineArea != null ? e.MineArea.Name : null,
                e.AssessmentNumber,
                e.Title,
                e.AssessmentType,
                e.Date,
                e.AssessorName,
                e.Location,
                e.RockMassRating,
                e.SlopeAngle,
                e.WaterTableDepth,
                e.GroundCondition,
                e.StabilityStatus,
                e.RecommendedActions,
                e.MonitoringRequired,
                e.NextAssessmentDate,
                e.Notes,
                e.Status,
                e.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
