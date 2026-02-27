using CoreEngine.Application.Features.Emails.Commands.ProcessEmailQueue;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreEngine.Infrastructure.BackgroundJobs;

public class ProcessEmailQueueJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessEmailQueueJob> _logger;

    public ProcessEmailQueueJob(IMediator mediator, ILogger<ProcessEmailQueueJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("ProcessEmailQueueJob started");
        try
        {
            var count = await _mediator.Send(new ProcessEmailQueueCommand());
            _logger.LogInformation("ProcessEmailQueueJob completed. Sent {Count} emails", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProcessEmailQueueJob failed");
        }
    }
}
