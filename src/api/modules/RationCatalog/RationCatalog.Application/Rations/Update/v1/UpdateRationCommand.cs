using MediatR;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Update.v1;
public sealed record UpdateRationCommand(
    Guid Id,
    string? Name,
    decimal DollarsPerHeadPerDay,
    string? Description = null) : IRequest<UpdateRationResponse>;

