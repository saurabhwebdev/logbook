using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Geotechnical.Commands.DeleteSurveyRecord;

public record DeleteSurveyRecordCommand(Guid Id) : IRequest;

public class DeleteSurveyRecordCommandHandler : IRequestHandler<DeleteSurveyRecordCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSurveyRecordCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSurveyRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.SurveyRecords
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Survey record {request.Id} not found.");

        _context.SurveyRecords.Remove(record);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
