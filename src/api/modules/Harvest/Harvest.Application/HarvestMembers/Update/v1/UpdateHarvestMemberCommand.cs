using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Update.v1;
public sealed record UpdateHarvestMemberCommand(
    Guid Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
) : IRequest<UpdateHarvestMemberResponse>;

