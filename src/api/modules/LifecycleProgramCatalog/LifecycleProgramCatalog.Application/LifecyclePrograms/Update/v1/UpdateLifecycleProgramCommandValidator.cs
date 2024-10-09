using FluentValidation;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
public class UpdateLifecycleProgramCommandValidator : AbstractValidator<UpdateLifecycleProgramCommand>
{
    public UpdateLifecycleProgramCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.Rating).GreaterThan(0);
    }
}

