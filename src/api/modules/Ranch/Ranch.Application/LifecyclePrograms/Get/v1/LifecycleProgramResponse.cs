using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;

public sealed record LifecycleProgramResponse(
    Guid? Id,
    string Name,
    string? Description,
    List<LifecycleProgramLifecycleStage> LifecycleProgramLifecycleStages
);