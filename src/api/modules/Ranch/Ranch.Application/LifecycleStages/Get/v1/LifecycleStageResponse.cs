using FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;

public sealed record LifecycleStageResponse(
    Guid? Id,
    string Name,
    string? Description,
    RationResponse Ration,
    GrowthTreatmentResponse GrowthTreatment,
    PreventiveTreatmentResponse PreventiveTreatment
);