
namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Create.v1;
public sealed record CreateContractMemberPageResponse(
    Guid? ContractId,
    Guid? MemberPageId
);
