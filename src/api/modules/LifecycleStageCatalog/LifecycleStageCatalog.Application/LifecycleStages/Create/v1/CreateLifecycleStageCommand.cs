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
    [property: DefaultValue("Sample LifecycleStage")] 
    string? Name,
    [property: DefaultValue(10)] 
    decimal Rating,
    [property: DefaultValue("Descriptive Description")] 
    string? Description = null,
    //TODO nullable?
    [property: JsonPropertyName("ration")]
    UpdateRationCommand? UpdateRationCommand = null,
    [property: JsonPropertyName("growthTreatment")]
    UpdateGrowthTreatmentCommand? UpdateGrowthTreatmentCommand = null,
    [property: JsonPropertyName("preventativeTreatment")]
    UpdatePreventativeTreatmentCommand? UpdatePreventativeTreatmentCommand = null
) : IRequest<CreateLifecycleStageResponse>;

