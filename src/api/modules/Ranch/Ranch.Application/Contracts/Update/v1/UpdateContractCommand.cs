using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Update.v1;
public sealed record UpdateContractCommand(
    Guid Id,
    string? TenantId,
    Guid? MemberId,

    List<UpdateContractMemberPageCommand> ContractMemberPages,
    string Name,
    string? Description,
    Guid? ContractStatusId
) : IRequest<UpdateContractResponse>;

