using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Delete.v1;
public sealed record DeleteMemberPageCommand(
    Guid Id) : IRequest;

