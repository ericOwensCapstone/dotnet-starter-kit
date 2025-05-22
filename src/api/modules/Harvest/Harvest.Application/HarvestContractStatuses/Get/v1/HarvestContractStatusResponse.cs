using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;

public sealed record HarvestContractStatusResponse(
    Guid? Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
);