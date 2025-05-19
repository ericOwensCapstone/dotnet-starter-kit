
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Create.v1;
public sealed record CreateContractMemberPageCommand(
    Guid? ContractId,
    Guid? MemberPageId
) : IRequest<CreateContractMemberPageResponse>;
