using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Create.v1;
public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
        RuleFor(p => p.ContractStatusId).NotEmpty();
    }
}

