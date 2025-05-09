using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Search.v1;

public class SearchMemberAdsCommand : PaginationFilter, IRequest<PagedList<MemberAdResponse>>
{
    //TODO Member Start
    public string TenantId { get; set; }
    //TODO Member End
}

