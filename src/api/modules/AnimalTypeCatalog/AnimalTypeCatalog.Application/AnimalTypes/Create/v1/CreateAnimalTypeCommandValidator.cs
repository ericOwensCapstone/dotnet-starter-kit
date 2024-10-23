using FluentValidation;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Create.v1;
public class CreateAnimalTypeCommandValidator : AbstractValidator<CreateAnimalTypeCommand>
{
    public CreateAnimalTypeCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}


