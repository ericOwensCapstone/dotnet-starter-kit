using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Search.v1;

public class SearchHarvestMembersCommand : PaginationFilter, IRequest<PagedList<HarvestMemberResponse>>
{
    //Start Custom Fields//End Custom Fields
    //Start One Per Code
    public string TenantId { get; set; }    
    //End One Per Code
}

