using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.ApiIntegration.Queries.GetWebhooks;

public record WebhookDto(Guid Id, string Name, string EndpointUrl, string EventTypes, bool IsActive, DateTime? LastTriggeredAt, int FailureCount, DateTime CreatedAt);

public record GetWebhooksQuery : IRequest<List<WebhookDto>>;

public class GetWebhooksQueryHandler : IRequestHandler<GetWebhooksQuery, List<WebhookDto>>
{
    private readonly IApplicationDbContext _context;
    public GetWebhooksQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<WebhookDto>> Handle(GetWebhooksQuery request, CancellationToken ct)
    {
        return await _context.WebhookSubscriptions.OrderByDescending(w => w.CreatedAt)
            .Select(w => new WebhookDto(w.Id, w.Name, w.EndpointUrl, w.EventTypes, w.IsActive, w.LastTriggeredAt, w.FailureCount, w.CreatedAt))
            .ToListAsync(ct);
    }
}
