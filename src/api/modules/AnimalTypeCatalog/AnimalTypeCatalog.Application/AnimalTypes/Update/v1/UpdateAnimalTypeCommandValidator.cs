using FluentValidation;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Update.v1;
public class UpdateAnimalTypeCommandValidator : AbstractValidator<UpdateAnimalTypeCommand>
{
    public UpdateAnimalTypeCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}


