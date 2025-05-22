using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Search.v1;
public class SearchHarvestContractStatusSpecs : EntitiesByPaginationFilterSpec<HarvestContractStatus, HarvestContractStatusResponse>
{
    public SearchHarvestContractStatusSpecs(SearchHarvestContractStatusesCommand command)
        : base(command) =>
        Query
            //Start One Per Code//End One Per Code  
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

