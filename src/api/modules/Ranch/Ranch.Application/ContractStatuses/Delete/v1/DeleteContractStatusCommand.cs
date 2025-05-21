using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Delete.v1;
public sealed record DeleteContractStatusCommand(
    Guid Id) : IRequest;

