using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Workflows.Commands.CancelWorkflow;

public class CancelWorkflowCommandHandler : IRequestHandler<CancelWorkflowCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeService _dateTimeService;

    public CancelWorkflowCommandHandler(
        IApplicationDbContext context,
        IDateTimeService dateTimeService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
    }

    public async Task Handle(CancelWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowInstance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(w => w.Id == request.WorkflowInstanceId, cancellationToken);

        if (workflowInstance == null)
            throw new NotFoundException(nameof(WorkflowInstance), request.WorkflowInstanceId);

        workflowInstance.Status = "Cancelled";
        workflowInstance.CompletedAt = _dateTimeService.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
