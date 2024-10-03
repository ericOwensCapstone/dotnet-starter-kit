using FluentValidation;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Create.v1;
public class CreateRationCommandValidator : AbstractValidator<CreateRationCommand>
{
    public CreateRationCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.DollarsPerPound).GreaterThan(0);
    }
}

