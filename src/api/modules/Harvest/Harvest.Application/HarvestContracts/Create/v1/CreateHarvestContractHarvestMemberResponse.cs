
namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Create.v1;
public sealed record CreateHarvestContractHarvestMemberResponse(
    Guid? HarvestContractId,
    Guid? HarvestMemberId
);
