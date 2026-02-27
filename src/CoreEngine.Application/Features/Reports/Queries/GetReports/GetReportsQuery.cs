using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Reports.Queries.GetReports;

public record ReportDto(Guid Id, string Name, string? Description, string EntityType, string ExportFormat, bool IsActive, DateTime CreatedAt);

public record GetReportsQuery : IRequest<List<ReportDto>>;

public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, List<ReportDto>>
{
    private readonly IApplicationDbContext _context;
    public GetReportsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ReportDto>> Handle(GetReportsQuery request, CancellationToken ct)
    {
        return await _context.ReportDefinitions.OrderBy(r => r.Name)
            .Select(r => new ReportDto(r.Id, r.Name, r.Description, r.EntityType, r.ExportFormat, r.IsActive, r.CreatedAt))
            .ToListAsync(ct);
    }
}
