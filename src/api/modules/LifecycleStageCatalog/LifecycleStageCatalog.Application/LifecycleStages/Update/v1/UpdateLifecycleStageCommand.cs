using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
public sealed record UpdateLifecycleStageCommand(
    Guid Id,
    string? Name,
    decimal Rating,
    string? Description = null) : IRequest<UpdateLifecycleStageResponse>;

