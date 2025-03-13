using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Update.v1;
public class UpdateRationCommandValidator : AbstractValidator<UpdateRationCommand>
{
    public UpdateRationCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
        RuleFor(p => p.DollarsPerPound).GreaterThan(0);
    }
}

