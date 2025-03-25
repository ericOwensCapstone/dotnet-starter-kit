using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;

public class GetLifecycleStageSpecs : Specification<LifecycleStage, LifecycleStageResponse>
{
    public GetLifecycleStageSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            // Includes are implemented in context model builder
        ;
    }
}

