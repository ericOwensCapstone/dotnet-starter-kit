using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Search.v1;

public class SearchContractStatusesCommand : PaginationFilter, IRequest<PagedList<ContractStatusResponse>>
{    
}

