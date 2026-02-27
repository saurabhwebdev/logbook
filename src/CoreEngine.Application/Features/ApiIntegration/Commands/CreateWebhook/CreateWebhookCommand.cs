using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;

namespace CoreEngine.Application.Features.ApiIntegration.Commands.CreateWebhook;

public record CreateWebhookResponse(Guid Id, string Secret);

public record CreateWebhookCommand(string Name, string EndpointUrl, string EventTypes) : IRequest<CreateWebhookResponse>;

public class CreateWebhookCommandHandler : IRequestHandler<CreateWebhookCommand, CreateWebhookResponse>
{
    private readonly IApplicationDbContext _context;
    public CreateWebhookCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<CreateWebhookResponse> Handle(CreateWebhookCommand request, CancellationToken ct)
    {
        var secret = $"whsec_{Guid.NewGuid():N}";

        var webhook = new WebhookSubscription
        {
            Name = request.Name,
            EndpointUrl = request.EndpointUrl,
            Secret = secret,
            EventTypes = request.EventTypes,
        };
        _context.WebhookSubscriptions.Add(webhook);
        await _context.SaveChangesAsync(ct);
        return new CreateWebhookResponse(webhook.Id, secret);
    }
}
