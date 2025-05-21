using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Search.v1;

public class SearchContractsCommand : PaginationFilter, IRequest<PagedList<ContractResponse>>
{
    //Start Custom Fields
    public Guid? ContractStatusId { get; set; }
    //End Custom Fields
    //Start One Per Code//End One Per Code
}

