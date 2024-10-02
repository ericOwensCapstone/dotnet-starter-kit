using MediatR;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Delete.v1;
public sealed record DeleteGrowthTreatmentCommand(
    Guid Id) : IRequest;

