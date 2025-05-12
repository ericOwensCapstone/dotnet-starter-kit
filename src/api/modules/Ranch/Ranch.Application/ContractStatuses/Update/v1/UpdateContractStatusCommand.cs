using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Update.v1;
public sealed record UpdateContractStatusCommand(
    Guid Id,
    string? TenantId,
    string Name,
    string? Description
) : IRequest<UpdateContractStatusResponse>;

