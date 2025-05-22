using FluentValidation;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Create.v1;
public class CreateHarvestMemberCommandValidator : AbstractValidator<CreateHarvestMemberCommand>
{
    public CreateHarvestMemberCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

