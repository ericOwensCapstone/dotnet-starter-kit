namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
public sealed record LifecycleStageResponse(Guid? Id, string Name, string? Description, decimal Rating);

