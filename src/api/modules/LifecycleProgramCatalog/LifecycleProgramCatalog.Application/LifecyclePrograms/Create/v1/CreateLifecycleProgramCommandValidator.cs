using FluentValidation;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Create.v1;
public class CreateLifecycleProgramCommandValidator : AbstractValidator<CreateLifecycleProgramCommand>
{
    public CreateLifecycleProgramCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}

