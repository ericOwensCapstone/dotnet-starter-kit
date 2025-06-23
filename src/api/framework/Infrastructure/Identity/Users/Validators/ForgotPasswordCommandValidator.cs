using FluentValidation;
using FSH.Framework.Core.Identity.Users.Abstractions;

namespace FSH.Framework.Infrastructure.Identity.Users.Validators;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}