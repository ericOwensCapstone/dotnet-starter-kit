using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Create.v1;
public class CreateContractStatusCommandValidator : AbstractValidator<CreateContractStatusCommand>
{
    public CreateContractStatusCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

