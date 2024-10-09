using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Create.v1;
public sealed record CreateLifecycleProgramCommand(
    [property: DefaultValue("Sample LifecycleProgram")] string? Name,
    [property: DefaultValue(10)] decimal Rating,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreateLifecycleProgramResponse>;

