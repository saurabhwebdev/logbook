using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Commands.DeleteGeotechnicalAssessment;

public record DeleteGeotechnicalAssessmentCommand(Guid Id) : IRequest;

public class DeleteGeotechnicalAssessmentCommandHandler : IRequestHandler<DeleteGeotechnicalAssessmentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteGeotechnicalAssessmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteGeotechnicalAssessmentCommand request, CancellationToken cancellationToken)
    {
        var assessment = await _context.GeotechnicalAssessments
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Geotechnical assessment {request.Id} not found.");

        _context.GeotechnicalAssessments.Remove(assessment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
