using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Search.v1;

public class SearchRationsCommand : PaginationFilter, IRequest<PagedList<RationResponse>>
{
    //Start Custom Fields
    //End Custom Fields
    //Start One Per Code
    //End One Per Code
}

