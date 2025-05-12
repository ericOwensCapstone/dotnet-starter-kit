using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Update.v1;
public class UpdateContractStatusCommandValidator : AbstractValidator<UpdateContractStatusCommand>
{
    public UpdateContractStatusCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

