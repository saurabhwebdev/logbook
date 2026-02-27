using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace CoreEngine.Application.Features.Reports.Commands.ExportReport;

public record ExportResultDto(byte[] FileContents, string ContentType, string FileName);

public record ExportReportCommand(Guid Id) : IRequest<ExportResultDto>;

public class ExportReportCommandHandler : IRequestHandler<ExportReportCommand, ExportResultDto>
{
    private readonly IApplicationDbContext _context;

    public ExportReportCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ExportResultDto> Handle(ExportReportCommand request, CancellationToken ct)
    {
        var report = await _context.ReportDefinitions.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("ReportDefinition", request.Id);

        var (headers, rows) = await GetEntityData(report.EntityType, ct);

        return report.ExportFormat switch
        {
            "Csv" => GenerateCsv(headers, rows, report.Name),
            "PDF" => GeneratePdf(headers, rows, report.Name),
            _ => GenerateExcel(headers, rows, report.Name)
        };
    }

    private async Task<(List<string> Headers, List<List<string>> Rows)> GetEntityData(string entityType, CancellationToken ct)
    {
        return entityType switch
        {
            "User" => await GetUserData(ct),
            "Department" => await GetDepartmentData(ct),
            "Role" => await GetRoleData(ct),
            "AuditLog" => await GetAuditLogData(ct),
            "DemoTask" => await GetDemoTaskData(ct),
            _ => (new List<string> { "Error" }, new List<List<string>> { new() { $"Unknown entity type: {entityType}" } })
        };
    }

    private async Task<(List<string>, List<List<string>>)> GetUserData(CancellationToken ct)
    {
        var headers = new List<string> { "Email", "First Name", "Last Name", "Status", "Created At" };
        var data = await _context.Users.OrderBy(u => u.Email)
            .Select(u => new List<string> { u.Email, u.FirstName, u.LastName, u.Status.ToString(), u.CreatedAt.ToString("yyyy-MM-dd") })
            .ToListAsync(ct);
        return (headers, data);
    }

    private async Task<(List<string>, List<List<string>>)> GetDepartmentData(CancellationToken ct)
    {
        var headers = new List<string> { "Name", "Code", "Created At" };
        var data = await _context.Departments.OrderBy(d => d.Name)
            .Select(d => new List<string> { d.Name, d.Code ?? "", d.CreatedAt.ToString("yyyy-MM-dd") })
            .ToListAsync(ct);
        return (headers, data);
    }

    private async Task<(List<string>, List<List<string>>)> GetRoleData(CancellationToken ct)
    {
        var headers = new List<string> { "Name", "Description", "System Role", "Created At" };
        var data = await _context.Roles.OrderBy(r => r.Name)
            .Select(r => new List<string> { r.Name, r.Description ?? "", r.IsSystemRole ? "Yes" : "No", r.CreatedAt.ToString("yyyy-MM-dd") })
            .ToListAsync(ct);
        return (headers, data);
    }

    private async Task<(List<string>, List<List<string>>)> GetAuditLogData(CancellationToken ct)
    {
        var headers = new List<string> { "Action", "Entity", "Entity ID", "User ID", "Timestamp" };
        var data = await _context.AuditLogs.OrderByDescending(a => a.Timestamp).Take(1000)
            .Select(a => new List<string> { a.Action, a.EntityName, a.EntityId, a.UserId ?? "", a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss") })
            .ToListAsync(ct);
        return (headers, data);
    }

    private async Task<(List<string>, List<List<string>>)> GetDemoTaskData(CancellationToken ct)
    {
        var headers = new List<string> { "Title", "State", "Priority", "Assigned To", "Created At" };
        var data = await _context.DemoTasks.OrderByDescending(t => t.CreatedAt)
            .Select(t => new List<string> { t.Title, t.CurrentState, t.Priority, t.AssignedTo ?? "", t.CreatedAt.ToString("yyyy-MM-dd") })
            .ToListAsync(ct);
        return (headers, data);
    }

    private static ExportResultDto GenerateExcel(List<string> headers, List<List<string>> rows, string reportName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        // Headers
        for (int i = 0; i < headers.Count; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
        }

        // Data rows
        for (int r = 0; r < rows.Count; r++)
        {
            for (int c = 0; c < rows[r].Count; c++)
            {
                worksheet.Cell(r + 2, c + 1).Value = rows[r][c];
            }
        }

        worksheet.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileName = $"{reportName.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.xlsx";
        return new ExportResultDto(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    private static ExportResultDto GenerateCsv(List<string> headers, List<List<string>> rows, string reportName)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", headers.Select(EscapeCsv)));
        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
        }
        var fileName = $"{reportName.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.csv";
        return new ExportResultDto(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", fileName);
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static ExportResultDto GeneratePdf(List<string> headers, List<List<string>> rows, string reportName)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Header
                page.Header().Element(ComposeHeader);

                // Content
                page.Content().Element(container => ComposeContent(container, headers, rows));

                // Footer with page numbers
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(reportName).FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text($"Generated on {DateTime.UtcNow:MMMM dd, yyyy HH:mm} UTC").FontSize(9).FontColor(Colors.Grey.Darken1);
                column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }

        void ComposeContent(IContainer container, List<string> headers, List<List<string>> rows)
        {
            container.Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    foreach (var _ in headers)
                    {
                        columns.RelativeColumn();
                    }
                });

                // Header row
                table.Header(header =>
                {
                    foreach (var headerText in headers)
                    {
                        header.Cell().Element(CellStyle).Background(Colors.Grey.Lighten2).Text(headerText).Bold().FontSize(9);
                    }
                });

                // Data rows
                foreach (var row in rows)
                {
                    foreach (var cell in row)
                    {
                        table.Cell().Element(CellStyle).Text(cell ?? "").FontSize(9);
                    }
                }

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
                }
            });
        }

        var pdfBytes = document.GeneratePdf();
        var fileName = $"{reportName.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.pdf";
        return new ExportResultDto(pdfBytes, "application/pdf", fileName);
    }
}
