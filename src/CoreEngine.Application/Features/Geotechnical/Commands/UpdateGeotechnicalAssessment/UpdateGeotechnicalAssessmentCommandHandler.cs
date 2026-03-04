using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Commands.UpdateGeotechnicalAssessment;

public class UpdateGeotechnicalAssessmentCommandHandler : IRequestHandler<UpdateGeotechnicalAssessmentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateGeotechnicalAssessmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateGeotechnicalAssessmentCommand request, CancellationToken cancellationToken)
    {
        var assessment = await _context.GeotechnicalAssessments
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Geotechnical assessment {request.Id} not found.");

        assessment.Title = request.Title;
        assessment.AssessmentType = request.AssessmentType;
        assessment.Date = request.Date;
        assessment.AssessorName = request.AssessorName;
        assessment.Location = request.Location;
        assessment.RockMassRating = request.RockMassRating;
        assessment.SlopeAngle = request.SlopeAngle;
        assessment.WaterTableDepth = request.WaterTableDepth;
        assessment.GroundCondition = request.GroundCondition;
        assessment.StabilityStatus = request.StabilityStatus;
        assessment.RecommendedActions = request.RecommendedActions;
        assessment.MonitoringRequired = request.MonitoringRequired;
        assessment.NextAssessmentDate = request.NextAssessmentDate;
        assessment.Notes = request.Notes;
        assessment.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
