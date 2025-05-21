using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;

public class GetContractStatusSpecs : Specification<ContractStatus, ContractStatusResponse>
{
    public GetContractStatusSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

