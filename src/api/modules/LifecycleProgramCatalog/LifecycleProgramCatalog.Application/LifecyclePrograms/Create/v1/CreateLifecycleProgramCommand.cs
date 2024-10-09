using System.ComponentModel;
using System.Text.Json.Serialization;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Create.v1;
public sealed record CreateLifecycleProgramCommand(
    [property: DefaultValue("Sample LifecycleProgram")] 
    string? Name,
    [property: DefaultValue(10)] 
    decimal Rating,
    [property: DefaultValue("Descriptive Description")] 
    string? Description = null,
    [property: JsonPropertyName("lifecycleStages")] 
    List<UpdateLifecycleStageCommand>? LifecycleStages = null
) : IRequest<CreateLifecycleProgramResponse>;

