
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Create.v1;
public sealed record CreateLifecycleProgramLifecycleStageCommand(
    Guid? LifecycleProgramId,
    Guid? LifecycleStageId,
    int Order
) : IRequest<CreateLifecycleProgramLifecycleStageResponse>;
