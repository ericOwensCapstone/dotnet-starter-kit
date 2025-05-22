using FluentValidation;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Update.v1;
public class UpdateHarvestContractCommandValidator : AbstractValidator<UpdateHarvestContractCommand>
{
    public UpdateHarvestContractCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
        RuleFor(p => p.HarvestContractStatusId).NotEmpty();
    }
}

