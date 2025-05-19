using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Create.v1;

public sealed record CreateMemberPageCommand(
    string? TenantId,
    Guid? MemberId,
    string? Name,
    string? Description
) : IRequest<CreateMemberPageResponse>;

