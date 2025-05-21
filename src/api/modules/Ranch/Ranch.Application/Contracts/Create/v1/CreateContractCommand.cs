using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Create.v1;

public sealed record CreateContractCommand(
    string? TenantId,
    Guid? MemberId,
    List<CreateContractMemberPageCommand> ContractMemberPages,
    string? Name,
    string? Description,
    Guid? ContractStatusId
) : IRequest<CreateContractResponse>;

