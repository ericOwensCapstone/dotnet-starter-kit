using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Create.v1;
public sealed record CreateRationCommand(
    [property: DefaultValue("Sample Ration")] string? Name,
    [property: DefaultValue(10)] decimal DollarsPerHeadPerDay,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreateRationResponse>;

