using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;

public sealed record HarvestContractResponse(
    Guid? Id,
    string? TenantId,
    Guid? MemberId,
    List<HarvestContractHarvestMember> HarvestContractHarvestMembers,
    string Name,
    string? Description,
    HarvestContractStatusResponse HarvestContractStatus
);