using FluentValidation;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Update.v1;
public class UpdateMemberAdCommandValidator : AbstractValidator<UpdateMemberAdCommand>
{
    public UpdateMemberAdCommandValidator()
    {
        RuleFor(p => p.TenantId).NotEmpty();
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

