using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Delete.v1;
public sealed record DeleteHarvestMemberCommand(
    Guid Id) : IRequest;

