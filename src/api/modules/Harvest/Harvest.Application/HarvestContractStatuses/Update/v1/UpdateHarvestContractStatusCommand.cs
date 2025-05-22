using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Update.v1;
public sealed record UpdateHarvestContractStatusCommand(
    Guid Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
) : IRequest<UpdateHarvestContractStatusResponse>;

