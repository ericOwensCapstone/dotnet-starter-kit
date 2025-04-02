using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Create.v1;
public class CreateLifecycleProgramCommandValidator : AbstractValidator<CreateLifecycleProgramCommand>
{
    public CreateLifecycleProgramCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(1000);
        RuleFor(p => p.LifecycleProgramLifecycleStages).NotEmpty();
    }
}

