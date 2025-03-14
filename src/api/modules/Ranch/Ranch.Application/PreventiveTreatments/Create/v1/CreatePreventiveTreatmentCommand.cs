using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Create.v1;
public sealed record CreatePreventiveTreatmentCommand(
    [property: DefaultValue("Sample PreventiveTreatment")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description,
    [property: DefaultValue(1.10)] decimal DollarsPerHead
) : IRequest<CreatePreventiveTreatmentResponse>;

