using Ardalis.Specification;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;

public class GetHarvestContractSpecs : Specification<HarvestContract, HarvestContractResponse>
{
    public GetHarvestContractSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

