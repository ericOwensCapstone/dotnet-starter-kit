using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Exceptions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Delete.v1;
public sealed class DeleteHarvestContractStatusHandler(
    ILogger<DeleteHarvestContractStatusHandler> logger,
    [FromKeyedServices("harvest:harvestContractStatuses")] IRepository<HarvestContractStatus> repository)
    : IRequestHandler<DeleteHarvestContractStatusCommand>
{
    public async Task Handle(DeleteHarvestContractStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var harvestContractStatus = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = harvestContractStatus ?? throw new HarvestContractStatusNotFoundException(request.Id);
        await repository.DeleteAsync(harvestContractStatus, cancellationToken);
        logger.LogInformation("harvestContractStatus with id : {HarvestContractStatusId} deleted", harvestContractStatus.Id);
    }
}

