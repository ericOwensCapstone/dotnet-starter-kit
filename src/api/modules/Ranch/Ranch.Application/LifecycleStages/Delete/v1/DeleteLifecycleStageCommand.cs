using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Delete.v1;
public sealed record DeleteLifecycleStageCommand(
    Guid Id) : IRequest;

