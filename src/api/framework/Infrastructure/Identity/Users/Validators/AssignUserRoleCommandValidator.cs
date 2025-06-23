using FluentValidation;
using FSH.Framework.Core.Identity.Users.Abstractions;

namespace FSH.Framework.Infrastructure.Identity.Users.Validators;

public class AssignUserRoleCommandValidator : AbstractValidator<AssignUserRoleCommand>
{
    public AssignUserRoleCommandValidator()
    {
        RuleFor(x => x.UserRoles)
            .NotNull().WithMessage("User roles list is required.")
            .NotEmpty().WithMessage("At least one role must be specified.");

        RuleForEach(x => x.UserRoles).ChildRules(role =>
        {
            role.RuleFor(r => r.RoleName)
                .NotEmpty().WithMessage("Role name is required.");
        });
    }
}