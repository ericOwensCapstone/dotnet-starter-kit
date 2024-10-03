using FluentValidation;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Create.v1;
public class CreatePreventativeTreatmentCommandValidator : AbstractValidator<CreatePreventativeTreatmentCommand>
{
    public CreatePreventativeTreatmentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.DollarsPerHead).GreaterThan(0);
    }
}

