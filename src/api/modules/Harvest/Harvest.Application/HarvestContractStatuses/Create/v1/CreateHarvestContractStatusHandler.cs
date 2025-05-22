using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Create.v1;
public sealed class CreateHarvestContractStatusHandler(
    //Start Injections//End Injections
    ILogger<CreateHarvestContractStatusHandler> logger,
    [FromKeyedServices("harvest:harvestContractStatuses")] IRepository<HarvestContractStatus> repository)
    : IRequestHandler<CreateHarvestContractStatusCommand, CreateHarvestContractStatusResponse>
{
    public async Task<CreateHarvestContractStatusResponse> Handle(CreateHarvestContractStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        //Start One Per Code//End One Per Code

        var harvestContractStatus = HarvestContractStatus.Create(
            request.TenantId,
            request.MemberId,
            request.Name,
            request.Description
        );
        
        await repository.AddAsync(harvestContractStatus, cancellationToken);
        logger.LogInformation("harvestContractStatus created {HarvestContractStatusId}", harvestContractStatus.Id);
        return new CreateHarvestContractStatusResponse(harvestContractStatus.Id);
    }
}

