using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Create.v1;

public sealed record CreateMemberAdCommand(
    string? TenantId,
    string? Name,
    string? Description
) : IRequest<CreateMemberAdResponse>;

