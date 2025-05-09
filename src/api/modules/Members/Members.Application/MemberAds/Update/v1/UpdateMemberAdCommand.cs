using MediatR;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Update.v1;
public sealed record UpdateMemberAdCommand(
    Guid Id,
    string? TenantId,
    string Name,
    string? Description
) : IRequest<UpdateMemberAdResponse>;

