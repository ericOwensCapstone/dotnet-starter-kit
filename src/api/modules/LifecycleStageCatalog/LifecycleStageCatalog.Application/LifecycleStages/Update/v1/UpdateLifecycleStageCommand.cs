using System.Text.Json.Serialization;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Update.v1;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Update.v1;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Update.v1;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
public sealed record UpdateLifecycleStageCommand(
    Guid Id,
    string? Name,
    decimal Rating,
    //TODO nullable?
    [property: JsonPropertyName("ration")]
    UpdateRationCommand? UpdateRationCommand,
    [property: JsonPropertyName("growthTreatment")]
    UpdateGrowthTreatmentCommand? UpdateGrowthTreatmentCommand,
    [property: JsonPropertyName("preventativeTreatment")]
    UpdatePreventativeTreatmentCommand? UpdatePreventativeTreatmentCommand,
    string? Description = null) : IRequest<UpdateLifecycleStageResponse>;

