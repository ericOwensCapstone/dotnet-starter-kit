using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Search.v1;

public class SearchHarvestContractsCommand : PaginationFilter, IRequest<PagedList<HarvestContractResponse>>
{
    //Start Custom Fields
    public Guid? HarvestContractStatusId { get; set; }
    //End Custom Fields
    //Start One Per Code//End One Per Code
}

