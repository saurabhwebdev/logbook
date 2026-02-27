using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Emails.Queries.GetEmailQueue;

public record EmailQueueDto(
    Guid Id,
    string To,
    string Subject,
    EmailStatus Status,
    DateTime? SentAt,
    string? FailureReason,
    int RetryCount,
    DateTime CreatedAt
);

public record GetEmailQueueQuery(EmailStatus? Status = null) : IRequest<List<EmailQueueDto>>;

public class GetEmailQueueQueryHandler : IRequestHandler<GetEmailQueueQuery, List<EmailQueueDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEmailQueueQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<EmailQueueDto>> Handle(GetEmailQueueQuery request, CancellationToken ct)
    {
        var query = _context.EmailQueues.Where(e => !e.IsDeleted);

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Take(200)
            .Select(e => new EmailQueueDto(e.Id, e.To, e.Subject, e.Status, e.SentAt, e.FailureReason, e.RetryCount, e.CreatedAt))
            .ToListAsync(ct);
    }
}
