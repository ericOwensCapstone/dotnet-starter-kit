
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Update.v1;
public sealed record UpdateContractMemberPageCommand(
    Guid? ContractId,
    Guid? MemberPageId
) : IRequest<UpdateContractMemberPageResponse>;
