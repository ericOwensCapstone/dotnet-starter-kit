
namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Update.v1;
public sealed record UpdateHarvestContractHarvestMemberResponse(
    Guid? HarvestContractId,
    Guid? HarvestMemberId
);
