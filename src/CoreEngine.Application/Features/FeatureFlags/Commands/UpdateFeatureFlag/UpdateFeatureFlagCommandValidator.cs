using FluentValidation;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.UpdateFeatureFlag;

public class UpdateFeatureFlagCommandValidator : AbstractValidator<UpdateFeatureFlagCommand>
{
    public UpdateFeatureFlagCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Feature flag ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Feature flag name is required.")
            .MaximumLength(200).WithMessage("Feature flag name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
