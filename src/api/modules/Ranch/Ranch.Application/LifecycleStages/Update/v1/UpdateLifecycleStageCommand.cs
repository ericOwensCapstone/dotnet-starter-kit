using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Update.v1;
public sealed record UpdateLifecycleStageCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? RationId,
    Guid? GrowthTreatmentId,
    Guid? PreventiveTreatmentId
) : IRequest<UpdateLifecycleStageResponse>;

