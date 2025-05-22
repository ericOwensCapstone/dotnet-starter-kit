using FluentValidation;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Create.v1;
public class CreateHarvestContractCommandValidator : AbstractValidator<CreateHarvestContractCommand>
{
    public CreateHarvestContractCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
        RuleFor(p => p.HarvestContractStatusId).NotEmpty();
    }
}

