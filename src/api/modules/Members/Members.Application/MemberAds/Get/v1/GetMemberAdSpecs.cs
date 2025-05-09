using Ardalis.Specification;
using FSH.Starter.WebApi.Members.Domain.MemberAds;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;

public class GetMemberAdSpecs : Specification<MemberAd, MemberAdResponse>
{
    public GetMemberAdSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

