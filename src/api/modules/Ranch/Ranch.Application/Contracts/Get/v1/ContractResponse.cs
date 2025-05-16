using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;

public sealed record ContractResponse(
    Guid? Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description,
    ContractStatusResponse ContractStatus
);