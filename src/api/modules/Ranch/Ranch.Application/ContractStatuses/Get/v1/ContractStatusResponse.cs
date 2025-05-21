using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;

public sealed record ContractStatusResponse(
    Guid? Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
);