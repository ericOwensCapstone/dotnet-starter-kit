using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Update.v1;
public sealed record UpdateHarvestContractCommand(
    Guid Id,
    string? TenantId,
    Guid? MemberId,

    List<UpdateHarvestContractHarvestMemberCommand> HarvestContractHarvestMembers,
    string Name,
    string? Description,
    Guid? HarvestContractStatusId
) : IRequest<UpdateHarvestContractResponse>;

