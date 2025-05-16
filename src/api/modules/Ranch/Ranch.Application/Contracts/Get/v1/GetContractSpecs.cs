using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;

public class GetContractSpecs : Specification<Contract, ContractResponse>
{
    public GetContractSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

