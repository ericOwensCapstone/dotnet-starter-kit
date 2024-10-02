using FluentValidation;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Create.v1;
public class CreateGrowthTreatmentCommandValidator : AbstractValidator<CreateGrowthTreatmentCommand>
{
    public CreateGrowthTreatmentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.DollarsPerHead).GreaterThan(0);
    }
}

