using System.ComponentModel;
using System.Text.Json.Serialization;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Update.v1;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Update.v1;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Update.v1;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
public sealed record CreateLifecycleStageCommand(
    double TargetWeight,
    double TargetAdfi,
    double AdfiStdDev,
    double TargetWeightRangeForSort,
    int MergeableDuration,
    double MergeableWeightRange,
    int MaxHead,
    [property: DefaultValue("Sample LifecycleStage")] 
    string? Name,
    [property: DefaultValue("Descriptive Description")] 
    string? Description = null,
    [property: JsonPropertyName("ration")]
    UpdateRationCommand? UpdateRationCommand = null,
    [property: JsonPropertyName("growthTreatment")]
    UpdateGrowthTreatmentCommand? UpdateGrowthTreatmentCommand = null,
    [property: JsonPropertyName("preventativeTreatment")]
    UpdatePreventativeTreatmentCommand? UpdatePreventativeTreatmentCommand = null
) : IRequest<CreateLifecycleStageResponse>;

