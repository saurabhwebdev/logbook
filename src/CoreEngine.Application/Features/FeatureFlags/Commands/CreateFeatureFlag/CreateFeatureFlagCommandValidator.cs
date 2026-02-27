using FluentValidation;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.CreateFeatureFlag;

public class CreateFeatureFlagCommandValidator : AbstractValidator<CreateFeatureFlagCommand>
{
    public CreateFeatureFlagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Feature flag name is required.")
            .MaximumLength(200).WithMessage("Feature flag name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
