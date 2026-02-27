using FluentValidation;

namespace CoreEngine.Application.Features.Workflows.Commands.CompleteTask;

public class CompleteTaskCommandValidator : AbstractValidator<CompleteTaskCommand>
{
    public CompleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => new[] { "Approved", "Rejected", "Completed" }.Contains(s))
            .WithMessage("Status must be 'Approved', 'Rejected', or 'Completed'.");
    }
}
