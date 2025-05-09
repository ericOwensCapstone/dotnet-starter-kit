using FluentValidation;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Create.v1;
public class CreateMemberAdCommandValidator : AbstractValidator<CreateMemberAdCommand>
{
    public CreateMemberAdCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

