using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Delete.v1;
public sealed record DeleteHarvestContractStatusCommand(
    Guid Id) : IRequest;

