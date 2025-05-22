using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Exceptions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Delete.v1;
public sealed class DeleteHarvestContractHandler(
    ILogger<DeleteHarvestContractHandler> logger,
    [FromKeyedServices("harvest:harvestContracts")] IRepository<HarvestContract> repository)
    : IRequestHandler<DeleteHarvestContractCommand>
{
    public async Task Handle(DeleteHarvestContractCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var harvestContract = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = harvestContract ?? throw new HarvestContractNotFoundException(request.Id);
        await repository.DeleteAsync(harvestContract, cancellationToken);
        logger.LogInformation("harvestContract with id : {HarvestContractId} deleted", harvestContract.Id);
    }
}

