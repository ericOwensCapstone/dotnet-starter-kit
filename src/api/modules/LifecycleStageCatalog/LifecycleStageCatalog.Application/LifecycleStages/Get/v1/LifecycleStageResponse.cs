using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
public sealed record LifecycleStageResponse(
    Guid? Id, 
    string Name, 
    string? Description, 
    Ration Ration, 
    GrowthTreatment GrowthTreatment, 
    PreventativeTreatment PreventativeTreatment,
    double TargetWeight,
    double TargetAdfi,
    double AdfiStdDev,
    double TargetWeightRangeForSort,
    int MergeableDuration,
    double MergeableWeightRange,
    int MaxHead
);

