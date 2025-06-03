using FluentValidation;

namespace FSH.Framework.Core.Identity.Invitations.Features;

public class CreateInvitationRequest
{
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string TenantId { get; set; } = default!;
    public string? Role { get; set; }
    public bool SendInvitationEmail { get; set; } = true;
    public string? CustomMessage { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class CreateInvitationRequestValidator : AbstractValidator<CreateInvitationRequest>
{
    public CreateInvitationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.TenantId)
            .NotEmpty();

        RuleFor(x => x.Role)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.Role));

        RuleFor(x => x.CustomMessage)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.CustomMessage));

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future");
    }
}