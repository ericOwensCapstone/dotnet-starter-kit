using Ardalis.Specification;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;

public class GetHarvestContractStatusSpecs : Specification<HarvestContractStatus, HarvestContractStatusResponse>
{
    public GetHarvestContractStatusSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

