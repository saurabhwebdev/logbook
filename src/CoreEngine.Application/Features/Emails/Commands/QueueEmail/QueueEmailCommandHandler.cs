using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Emails.Commands.QueueEmail;

public class QueueEmailCommandHandler : IRequestHandler<QueueEmailCommand, Unit>
{
    private readonly IEmailService _emailService;
    public QueueEmailCommandHandler(IEmailService emailService) => _emailService = emailService;

    public async Task<Unit> Handle(QueueEmailCommand request, CancellationToken ct)
    {
        await _emailService.QueueEmailAsync(request.To, request.Subject, request.HtmlBody, request.PlainTextBody, ct);
        return Unit.Value;
    }
}
