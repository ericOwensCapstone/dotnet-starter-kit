using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Update.v1;
public class UpdateMemberPageCommandValidator : AbstractValidator<UpdateMemberPageCommand>
{
    public UpdateMemberPageCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

