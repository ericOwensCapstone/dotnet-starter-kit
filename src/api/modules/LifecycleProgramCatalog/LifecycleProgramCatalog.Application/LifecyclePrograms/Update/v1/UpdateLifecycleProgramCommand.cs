using System.Text.Json.Serialization;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
public sealed record UpdateLifecycleProgramCommand
(
    Guid Id,
    string? Name,
    decimal Rating,
    string? Description = null,
    [property: JsonPropertyName("lifecycleStages")] 
    List<UpdateLifecycleStageCommand>? LifecycleStages = null
) : IRequest<UpdateLifecycleProgramResponse>;

