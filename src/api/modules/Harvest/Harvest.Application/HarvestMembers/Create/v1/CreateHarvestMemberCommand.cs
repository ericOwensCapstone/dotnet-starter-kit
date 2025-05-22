using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Create.v1;

public sealed record CreateHarvestMemberCommand(
    string? TenantId,
    Guid? MemberId,
    string? Name,
    string? Description
) : IRequest<CreateHarvestMemberResponse>;

