using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Delete.v1;
public sealed record DeleteContractCommand(
    Guid Id) : IRequest;

