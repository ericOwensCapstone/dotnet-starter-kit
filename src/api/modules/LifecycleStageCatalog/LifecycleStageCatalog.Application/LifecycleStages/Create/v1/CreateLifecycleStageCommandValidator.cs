using FluentValidation;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
public class CreateLifecycleStageCommandValidator : AbstractValidator<CreateLifecycleStageCommand>
{
    public CreateLifecycleStageCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}

