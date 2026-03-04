using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Commands.CreateGeotechnicalAssessment;

public class CreateGeotechnicalAssessmentCommandHandler : IRequestHandler<CreateGeotechnicalAssessmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateGeotechnicalAssessmentCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateGeotechnicalAssessmentCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.GeotechnicalAssessments.CountAsync(cancellationToken) + 1;
        var assessmentNumber = $"GEO-{count:D5}";

        var assessment = new GeotechnicalAssessment
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            AssessmentNumber = assessmentNumber,
            Title = request.Title,
            AssessmentType = request.AssessmentType,
            Date = request.Date,
            AssessorName = request.AssessorName,
            Location = request.Location,
            RockMassRating = request.RockMassRating,
            SlopeAngle = request.SlopeAngle,
            WaterTableDepth = request.WaterTableDepth,
            GroundCondition = request.GroundCondition,
            StabilityStatus = request.StabilityStatus,
            RecommendedActions = request.RecommendedActions,
            MonitoringRequired = request.MonitoringRequired,
            NextAssessmentDate = request.NextAssessmentDate,
            Notes = request.Notes,
            Status = "Draft",
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.GeotechnicalAssessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);

        return assessment.Id;
    }
}
