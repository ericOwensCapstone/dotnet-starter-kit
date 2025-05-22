using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Search.v1;

public class SearchHarvestContractStatusesCommand : PaginationFilter, IRequest<PagedList<HarvestContractStatusResponse>>
{
    //Start Custom Fields//End Custom Fields
    //Start One Per Code//End One Per Code
}

