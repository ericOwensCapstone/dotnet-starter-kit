using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Create.v1;
public sealed record CreateLifecycleProgramCommand(
    [property: DefaultValue("Sample LifecycleProgram")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description,
    List<CreateLifecycleProgramLifecycleStageCommand> LifecycleProgramLifecycleStages
) : IRequest<CreateLifecycleProgramResponse>;

