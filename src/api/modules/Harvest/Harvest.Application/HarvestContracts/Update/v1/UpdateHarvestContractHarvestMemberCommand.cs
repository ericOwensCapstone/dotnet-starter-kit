
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Update.v1;
public sealed record UpdateHarvestContractHarvestMemberCommand(
    Guid? HarvestContractId,
    Guid? HarvestMemberId
) : IRequest<UpdateHarvestContractHarvestMemberResponse>;
