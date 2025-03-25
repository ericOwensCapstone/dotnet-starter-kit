using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Create.v1;
public sealed record CreateLifecycleStageCommand(
    [property: DefaultValue("Sample LifecycleStage")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description,
    Guid? RationId,
    Guid? GrowthTreatmentId,
    Guid? PreventiveTreatmentId
) : IRequest<CreateLifecycleStageResponse>;

