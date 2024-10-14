using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
public sealed record LifecycleProgramResponse
(
    Guid? Id, 
    string Name, 
    string? Description, 
    decimal Rating,
    List<LifecycleProgramStageResponse>? LifecycleProgramStages
);

