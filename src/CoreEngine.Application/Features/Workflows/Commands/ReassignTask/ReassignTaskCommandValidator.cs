using FluentValidation;

namespace CoreEngine.Application.Features.Workflows.Commands.ReassignTask;

public class ReassignTaskCommandValidator : AbstractValidator<ReassignTaskCommand>
{
    public ReassignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.NewAssigneeUserId)
            .NotEmpty().WithMessage("New assignee user ID is required.");
    }
}
