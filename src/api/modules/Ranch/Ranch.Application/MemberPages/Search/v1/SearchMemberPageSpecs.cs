using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Search.v1;
public class SearchMemberPageSpecs : EntitiesByPaginationFilterSpec<MemberPage, MemberPageResponse>
{
    public SearchMemberPageSpecs(SearchMemberPagesCommand command)
        : base(command) =>
        Query
            //Start One Per Code
            .Where(p => p.TenantId == command.TenantId, command.TenantId != null)
            //End One Per Code  
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

