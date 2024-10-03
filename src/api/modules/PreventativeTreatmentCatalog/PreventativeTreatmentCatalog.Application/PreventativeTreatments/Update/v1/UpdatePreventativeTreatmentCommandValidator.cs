using FluentValidation;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Update.v1;
public class UpdatePreventativeTreatmentCommandValidator : AbstractValidator<UpdatePreventativeTreatmentCommand>
{
    public UpdatePreventativeTreatmentCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.DollarsPerHead).GreaterThan(0);
    }
}

