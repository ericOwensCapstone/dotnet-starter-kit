using MediatR;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Delete.v1;
public sealed record DeleteMemberAdCommand(
    Guid Id) : IRequest;

