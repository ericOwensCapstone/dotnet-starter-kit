using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Update.v1;
public class UpdateLifecycleStageCommandValidator : AbstractValidator<UpdateLifecycleStageCommand>
{
    public UpdateLifecycleStageCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(1000);
        RuleFor(p => p.RationId).NotEmpty();
        RuleFor(p => p.GrowthTreatmentId).NotEmpty();
        RuleFor(p => p.PreventiveTreatmentId).NotEmpty();
    }
}

