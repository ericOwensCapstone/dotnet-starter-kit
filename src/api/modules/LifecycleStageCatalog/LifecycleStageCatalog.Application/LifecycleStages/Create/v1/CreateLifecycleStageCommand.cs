using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
public sealed record CreateLifecycleStageCommand(
    [property: DefaultValue("Sample LifecycleStage")] string? Name,
    [property: DefaultValue(10)] decimal Rating,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreateLifecycleStageResponse>;

