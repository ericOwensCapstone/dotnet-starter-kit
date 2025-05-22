using FluentValidation;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Update.v1;
public class UpdateHarvestMemberCommandValidator : AbstractValidator<UpdateHarvestMemberCommand>
{
    public UpdateHarvestMemberCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

