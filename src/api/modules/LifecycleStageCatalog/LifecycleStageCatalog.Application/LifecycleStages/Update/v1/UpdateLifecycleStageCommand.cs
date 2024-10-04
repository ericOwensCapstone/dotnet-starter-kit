using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
public sealed record UpdateLifecycleStageCommand(
    Guid Id,
    string? Name,
    decimal Rating,
    Ration Ration,
    GrowthTreatment GrowthTreatment,
    PreventativeTreatment PreventativeTreatment,
    string? Description = null) : IRequest<UpdateLifecycleStageResponse>;

