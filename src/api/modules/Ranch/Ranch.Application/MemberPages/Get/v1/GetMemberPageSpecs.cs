using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;

public class GetMemberPageSpecs : Specification<MemberPage, MemberPageResponse>
{
    public GetMemberPageSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

