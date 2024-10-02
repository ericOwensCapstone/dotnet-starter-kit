using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Create.v1;
public sealed record CreateGrowthTreatmentCommand(
    [property: DefaultValue("Sample GrowthTreatment")] string? Name,
    [property: DefaultValue(10)] decimal DollarsPerHead,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreateGrowthTreatmentResponse>;

