using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.ApiIntegration.Commands.DeleteWebhook;

public record DeleteWebhookCommand(Guid Id) : IRequest;

public class DeleteWebhookCommandHandler : IRequestHandler<DeleteWebhookCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteWebhookCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteWebhookCommand request, CancellationToken ct)
    {
        var webhook = await _context.WebhookSubscriptions.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("WebhookSubscription", request.Id);
        _context.WebhookSubscriptions.Remove(webhook);
        await _context.SaveChangesAsync(ct);
    }
}
