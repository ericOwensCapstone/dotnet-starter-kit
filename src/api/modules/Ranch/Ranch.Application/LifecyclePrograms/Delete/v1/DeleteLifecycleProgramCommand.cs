using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Delete.v1;
public sealed record DeleteLifecycleProgramCommand(
    Guid Id) : IRequest;

