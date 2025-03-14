using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Create.v1;
public class CreatePreventiveTreatmentCommandValidator : AbstractValidator<CreatePreventiveTreatmentCommand>
{
    public CreatePreventiveTreatmentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(98);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(998);
        RuleFor(p => p.DollarsPerHead).GreaterThan(0);
    }
}

