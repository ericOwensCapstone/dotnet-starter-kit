using FluentValidation;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Update.v1;
public class UpdateRationCommandValidator : AbstractValidator<UpdateRationCommand>
{
    public UpdateRationCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.DollarsPerHeadPerDay).GreaterThan(0);
    }
}

