using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Update.v1;
public sealed record UpdateGrowthTreatmentCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal DollarsPerHead
) : IRequest<UpdateGrowthTreatmentResponse>;

