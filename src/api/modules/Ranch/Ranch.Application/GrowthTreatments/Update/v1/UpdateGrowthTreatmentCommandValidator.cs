using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Update.v1;
public class UpdateGrowthTreatmentCommandValidator : AbstractValidator<UpdateGrowthTreatmentCommand>
{
    public UpdateGrowthTreatmentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}

