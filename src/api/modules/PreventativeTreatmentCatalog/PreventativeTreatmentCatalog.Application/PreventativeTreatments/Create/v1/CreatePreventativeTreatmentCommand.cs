using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Create.v1;
public sealed record CreatePreventativeTreatmentCommand(
    [property: DefaultValue("Sample PreventativeTreatment")] string? Name,
    [property: DefaultValue(10)] decimal DollarsPerHead,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreatePreventativeTreatmentResponse>;

