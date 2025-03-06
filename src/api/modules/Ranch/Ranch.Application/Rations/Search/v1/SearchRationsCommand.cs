using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Search.v1;

public class SearchRationsCommand : PaginationFilter, IRequest<PagedList<RationResponse>>
{
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}

