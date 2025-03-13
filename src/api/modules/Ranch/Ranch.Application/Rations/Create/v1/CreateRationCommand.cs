using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Create.v1;
public sealed record CreateRationCommand(
    [property: DefaultValue("Sample Ration")] string? Name,
    [property: DefaultValue("Ration Description")] string? Description,
    [property: DefaultValue(0.10)] decimal DollarsPerPound
) : IRequest<CreateRationResponse>;

