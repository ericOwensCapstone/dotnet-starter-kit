using FluentValidation;

namespace FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;

public class ExtendInvitationExpirationValidator : AbstractValidator<ExtendInvitationExpirationRequest>
{
    public ExtendInvitationExpirationValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Invitation ID is required");

        RuleFor(x => x.NewExpirationDate)
            .NotEmpty()
            .WithMessage("New expiration date is required")
            .Must(BeInTheFuture)
            .WithMessage("New expiration date must be in the future")
            .Must(NotBeTooFarInFuture)
            .WithMessage("New expiration date cannot be more than 1 year in the future");
    }

    private static bool BeInTheFuture(DateTime date)
    {
        return date > DateTime.UtcNow.AddMinutes(5); // At least 5 minutes in the future
    }

    private static bool NotBeTooFarInFuture(DateTime date)
    {
        return date <= DateTime.UtcNow.AddDays(365); // Max 1 year extension
    }
}