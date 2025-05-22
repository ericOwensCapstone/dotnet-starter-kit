using FluentValidation;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Update.v1;
public class UpdateHarvestContractStatusCommandValidator : AbstractValidator<UpdateHarvestContractStatusCommand>
{
    public UpdateHarvestContractStatusCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

