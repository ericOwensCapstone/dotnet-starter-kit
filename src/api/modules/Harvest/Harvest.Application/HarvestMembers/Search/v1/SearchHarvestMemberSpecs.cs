using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Search.v1;
public class SearchHarvestMemberSpecs : EntitiesByPaginationFilterSpec<HarvestMember, HarvestMemberResponse>
{
    public SearchHarvestMemberSpecs(SearchHarvestMembersCommand command)
        : base(command) =>
        Query
            //Start One Per Code
            .Where(p => p.TenantId == command.TenantId, command.TenantId != null)
            //End One Per Code  
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

