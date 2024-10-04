using System.ComponentModel;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
public sealed record CreateLifecycleStageCommand(
    [property: DefaultValue("Sample LifecycleStage")] string? Name,
    [property: DefaultValue(10)] decimal Rating,
    [property: DefaultValue("Descriptive Description")] string? Description = null,
    Ration? Ration = null,
    GrowthTreatment? GrowthTreatment = null,
    PreventativeTreatment? PreventativeTreatment = null
) : IRequest<CreateLifecycleStageResponse>;

