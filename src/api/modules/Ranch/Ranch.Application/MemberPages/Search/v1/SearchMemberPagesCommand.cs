using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Search.v1;

public class SearchMemberPagesCommand : PaginationFilter, IRequest<PagedList<MemberPageResponse>>
{
    //Start Custom Fields//End Custom Fields
    //Start One Per Code
    public string TenantId { get; set; }    
    //End One Per Code
}

