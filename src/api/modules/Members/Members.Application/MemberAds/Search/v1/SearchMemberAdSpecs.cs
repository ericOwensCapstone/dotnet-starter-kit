using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
using FSH.Starter.WebApi.Members.Domain.MemberAds;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Search.v1;
public class SearchMemberAdSpecs : EntitiesByPaginationFilterSpec<MemberAd, MemberAdResponse>
{
    public SearchMemberAdSpecs(SearchMemberAdsCommand command)
        : base(command) =>
        Query
            // Includes are implemented in context model builder
            .OrderBy(c => c.Name, !command.HasOrderBy())
            //TODO Member Start
            .Where(p => p.TenantId == command.TenantId, command.TenantId != null);
            //TODO Member End
}

