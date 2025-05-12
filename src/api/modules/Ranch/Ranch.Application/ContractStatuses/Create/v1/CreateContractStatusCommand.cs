using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Create.v1;

public sealed record CreateContractStatusCommand(
    string? TenantId,
    string? Name,
    string? Description
) : IRequest<CreateContractStatusResponse>;

