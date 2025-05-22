
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Create.v1;
public sealed record CreateHarvestContractHarvestMemberCommand(
    Guid? HarvestContractId,
    Guid? HarvestMemberId
) : IRequest<CreateHarvestContractHarvestMemberResponse>;
