using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Update.v1;
public sealed record UpdateContractStatusCommand(
    Guid Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
) : IRequest<UpdateContractStatusResponse>;

