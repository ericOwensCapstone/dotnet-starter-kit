using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Create.v1;
public sealed record CreateGrowthTreatmentCommand(
    [property: DefaultValue("Sample GrowthTreatment")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description,
    [property: DefaultValue(1.10)] decimal DollarsPerHead
) : IRequest<CreateGrowthTreatmentResponse>;

