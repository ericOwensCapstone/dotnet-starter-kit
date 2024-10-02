using MediatR;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Update.v1;
public sealed record UpdateGrowthTreatmentCommand(
    Guid Id,
    string? Name,
    decimal DollarsPerHead,
    string? Description = null) : IRequest<UpdateGrowthTreatmentResponse>;

