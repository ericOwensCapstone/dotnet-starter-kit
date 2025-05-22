using Mapster;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Exceptions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Update.v1;
public sealed class UpdateHarvestContractHandler(
    ILogger<UpdateHarvestContractHandler> logger,
    [FromKeyedServices("harvest:harvestContracts")] IRepository<HarvestContract> repository)
    : IRequestHandler<UpdateHarvestContractCommand, UpdateHarvestContractResponse>
{
    public async Task<UpdateHarvestContractResponse> Handle(UpdateHarvestContractCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var harvestContractHarvestMembers = new List<HarvestContractHarvestMember>();
        foreach (var v in request.HarvestContractHarvestMembers)
        {
            var r = v.Adapt<HarvestContractHarvestMember>();
            harvestContractHarvestMembers.Add(r);
        }

        var harvestContract = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = harvestContract ?? throw new HarvestContractNotFoundException(request.Id);
        
        var updatedHarvestContract = harvestContract.Update(
            request.TenantId,
            request.MemberId,
            harvestContractHarvestMembers,
            request.Name,
            request.Description,
            request.HarvestContractStatusId
        );
        await repository.UpdateAsync(updatedHarvestContract, cancellationToken);
        logger.LogInformation("harvestContract with id : {HarvestContractId} updated.", harvestContract.Id);
        return new UpdateHarvestContractResponse(harvestContract.Id);
    }
}

