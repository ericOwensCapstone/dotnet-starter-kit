using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Update.v1;
public class UpdateLifecycleProgramCommandValidator : AbstractValidator<UpdateLifecycleProgramCommand>
{
    public UpdateLifecycleProgramCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(1000);
        RuleFor(p => p.LifecycleProgramLifecycleStages).NotEmpty();
    }
}

