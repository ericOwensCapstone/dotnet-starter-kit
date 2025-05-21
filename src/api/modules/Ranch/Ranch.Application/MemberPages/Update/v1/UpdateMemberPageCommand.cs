using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Update.v1;
public sealed record UpdateMemberPageCommand(
    Guid Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
) : IRequest<UpdateMemberPageResponse>;

