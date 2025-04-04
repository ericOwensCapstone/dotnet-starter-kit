using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;

public class GetLifecycleProgramSpecs : Specification<LifecycleProgram, LifecycleProgramResponse>
{
    public GetLifecycleProgramSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

