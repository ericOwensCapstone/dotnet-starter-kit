using Mapster;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Create.v1;
public sealed class CreateHarvestContractHandler(
    //Start Injections//End Injections
    ILogger<CreateHarvestContractHandler> logger,
    [FromKeyedServices("harvest:harvestContracts")] IRepository<HarvestContract> repository)
    : IRequestHandler<CreateHarvestContractCommand, CreateHarvestContractResponse>
{
    public async Task<CreateHarvestContractResponse> Handle(CreateHarvestContractCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        //Start One Per Code//End One Per Code

        var harvestContractHarvestMembers = new List<HarvestContractHarvestMember>();
        foreach (var v in request.HarvestContractHarvestMembers)
        {
            var r = v.Adapt<HarvestContractHarvestMember>();
            harvestContractHarvestMembers.Add(r);
        }

        var harvestContract = HarvestContract.Create(
            request.TenantId,
            request.MemberId,
            harvestContractHarvestMembers,
            request.Name,
            request.Description,
            request.HarvestContractStatusId
        );
        
        await repository.AddAsync(harvestContract, cancellationToken);
        logger.LogInformation("harvestContract created {HarvestContractId}", harvestContract.Id);
        return new CreateHarvestContractResponse(harvestContract.Id);
    }
}

