using Ardalis.Specification;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;

public class GetHarvestMemberSpecs : Specification<HarvestMember, HarvestMemberResponse>
{
    public GetHarvestMemberSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

