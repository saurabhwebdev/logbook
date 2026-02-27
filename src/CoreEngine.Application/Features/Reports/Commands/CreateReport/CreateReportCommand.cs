using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.Reports.Commands.CreateReport;

public record CreateReportCommand(string Name, string? Description, string EntityType, string ColumnsJson, string? FiltersJson, string ExportFormat) : IRequest<Guid>;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateReportCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateReportCommand request, CancellationToken ct)
    {
        var report = new ReportDefinition
        {
            Name = request.Name,
            Description = request.Description,
            EntityType = request.EntityType,
            ColumnsJson = request.ColumnsJson,
            FiltersJson = request.FiltersJson,
            ExportFormat = request.ExportFormat,
        };
        _context.ReportDefinitions.Add(report);
        await _context.SaveChangesAsync(ct);
        return report.Id;
    }
}
