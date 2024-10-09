using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
public sealed record UpdateLifecycleProgramCommand(
    Guid Id,
    string? Name,
    decimal Rating,
    string? Description = null) : IRequest<UpdateLifecycleProgramResponse>;

