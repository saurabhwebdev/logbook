using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Reports.Commands.DeleteReport;

public record DeleteReportCommand(Guid Id) : IRequest;

public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteReportCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteReportCommand request, CancellationToken ct)
    {
        var report = await _context.ReportDefinitions.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("ReportDefinition", request.Id);
        _context.ReportDefinitions.Remove(report);
        await _context.SaveChangesAsync(ct);
    }
}
