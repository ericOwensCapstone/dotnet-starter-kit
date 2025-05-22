using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Exceptions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Update.v1;
public sealed class UpdateHarvestContractStatusHandler(
    ILogger<UpdateHarvestContractStatusHandler> logger,
    [FromKeyedServices("harvest:harvestContractStatuses")] IRepository<HarvestContractStatus> repository)
    : IRequestHandler<UpdateHarvestContractStatusCommand, UpdateHarvestContractStatusResponse>
{
    public async Task<UpdateHarvestContractStatusResponse> Handle(UpdateHarvestContractStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var harvestContractStatus = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = harvestContractStatus ?? throw new HarvestContractStatusNotFoundException(request.Id);
        
        var updatedHarvestContractStatus = harvestContractStatus.Update(
            request.TenantId,
            request.MemberId,
            request.Name,
            request.Description
        );
        await repository.UpdateAsync(updatedHarvestContractStatus, cancellationToken);
        logger.LogInformation("harvestContractStatus with id : {HarvestContractStatusId} updated.", harvestContractStatus.Id);
        return new UpdateHarvestContractStatusResponse(harvestContractStatus.Id);
    }
}

