using FluentValidation;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
public class UpdateLifecycleStageCommandValidator : AbstractValidator<UpdateLifecycleStageCommand>
{
    public UpdateLifecycleStageCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}

