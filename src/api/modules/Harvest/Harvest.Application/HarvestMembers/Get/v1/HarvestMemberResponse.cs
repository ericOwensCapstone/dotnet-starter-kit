using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;

public sealed record HarvestMemberResponse(
    Guid? Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
);