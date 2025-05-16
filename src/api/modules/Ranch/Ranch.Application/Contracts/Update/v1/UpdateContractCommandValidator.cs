using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Update.v1;
public class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
{
    public UpdateContractCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
        RuleFor(p => p.ContractStatusId).NotEmpty();
    }
}

