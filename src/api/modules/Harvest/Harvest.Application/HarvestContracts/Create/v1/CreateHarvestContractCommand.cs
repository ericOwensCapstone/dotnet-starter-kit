using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Create.v1;

public sealed record CreateHarvestContractCommand(
    string? TenantId,
    Guid? MemberId,
    List<CreateHarvestContractHarvestMemberCommand> HarvestContractHarvestMembers,
    string? Name,
    string? Description,
    Guid? HarvestContractStatusId
) : IRequest<CreateHarvestContractResponse>;

