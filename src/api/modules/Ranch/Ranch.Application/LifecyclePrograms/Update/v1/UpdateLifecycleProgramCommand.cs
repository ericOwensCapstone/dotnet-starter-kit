using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Update.v1;
public sealed record UpdateLifecycleProgramCommand(
    Guid Id,
    string Name,
    string? Description,

    List<UpdateLifecycleProgramLifecycleStageCommand> LifecycleProgramLifecycleStages
) : IRequest<UpdateLifecycleProgramResponse>;

