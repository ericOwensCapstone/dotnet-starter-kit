using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Create.v1;
public class CreateGrowthTreatmentCommandValidator : AbstractValidator<CreateGrowthTreatmentCommand>
{
    public CreateGrowthTreatmentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
        RuleFor(p => p.DollarsPerHead).GreaterThan(0);
    }
}

