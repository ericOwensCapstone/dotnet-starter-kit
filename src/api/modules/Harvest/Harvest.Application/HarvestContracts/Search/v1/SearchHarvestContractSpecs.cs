using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Search.v1;
public class SearchHarvestContractSpecs : EntitiesByPaginationFilterSpec<HarvestContract, HarvestContractResponse>
{
    public SearchHarvestContractSpecs(SearchHarvestContractsCommand command)
        : base(command) =>
        Query
            //Start One Per Code//End One Per Code  
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

