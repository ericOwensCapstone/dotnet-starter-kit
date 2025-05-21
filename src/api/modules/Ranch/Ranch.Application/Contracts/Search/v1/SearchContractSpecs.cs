using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Search.v1;
public class SearchContractSpecs : EntitiesByPaginationFilterSpec<Contract, ContractResponse>
{
    public SearchContractSpecs(SearchContractsCommand command)
        : base(command) =>
        Query
            //Start One Per Code//End One Per Code  
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

