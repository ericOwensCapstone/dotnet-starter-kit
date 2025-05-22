using FluentValidation;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Create.v1;
public class CreateHarvestContractStatusCommandValidator : AbstractValidator<CreateHarvestContractStatusCommand>
{
    public CreateHarvestContractStatusCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

