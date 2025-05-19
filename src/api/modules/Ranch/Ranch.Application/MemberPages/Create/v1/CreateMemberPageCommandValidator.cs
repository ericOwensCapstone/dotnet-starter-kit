using FluentValidation;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Create.v1;
public class CreateMemberPageCommandValidator : AbstractValidator<CreateMemberPageCommand>
{
    public CreateMemberPageCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(99);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(2).MaximumLength(999);
    }
}

