using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Create.v1;

public sealed record CreateRationCommand(
    [property: DefaultValue("Sample Ration")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description,
    [property: DefaultValue(0.1)] decimal DollarsPerPound
) : IRequest<CreateRationResponse>;

