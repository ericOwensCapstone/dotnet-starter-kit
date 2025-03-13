using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Update.v1;
public sealed record UpdateRationCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal DollarsPerPound
) : IRequest<UpdateRationResponse>;

