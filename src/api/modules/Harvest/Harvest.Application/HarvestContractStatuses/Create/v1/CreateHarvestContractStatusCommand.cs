using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Create.v1;

public sealed record CreateHarvestContractStatusCommand(
    string? TenantId,
    Guid? MemberId,
    string? Name,
    string? Description
) : IRequest<CreateHarvestContractStatusResponse>;

