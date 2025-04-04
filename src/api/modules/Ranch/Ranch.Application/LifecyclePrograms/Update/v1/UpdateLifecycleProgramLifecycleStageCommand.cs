
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Update.v1;
public sealed record UpdateLifecycleProgramLifecycleStageCommand(
    Guid? LifecycleProgramId,
    Guid? LifecycleStageId,
    int Order
) : IRequest<UpdateLifecycleProgramLifecycleStageResponse>;
