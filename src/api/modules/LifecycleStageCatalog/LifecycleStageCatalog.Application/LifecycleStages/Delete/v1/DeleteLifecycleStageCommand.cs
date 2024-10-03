using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Delete.v1;
public sealed record DeleteLifecycleStageCommand(
    Guid Id) : IRequest;

