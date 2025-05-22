using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Delete.v1;
public sealed record DeleteHarvestContractCommand(
    Guid Id) : IRequest;

