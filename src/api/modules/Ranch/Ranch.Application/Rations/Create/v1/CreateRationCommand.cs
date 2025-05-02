using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Create.v1;

public sealed record CreateRationCommand(
    string? Name,
    string? Description,
    decimal DollarsPerPound
) : IRequest<CreateRationResponse>;

