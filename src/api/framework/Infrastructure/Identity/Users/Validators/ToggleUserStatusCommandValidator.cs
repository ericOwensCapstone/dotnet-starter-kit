using FluentValidation;
using FSH.Framework.Core.Identity.Users.Abstractions;

namespace FSH.Framework.Infrastructure.Identity.Users.Validators;

public class ToggleUserStatusCommandValidator : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}