using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Create.v1;
public sealed class CreateGrowthTreatmentHandler(
    ILogger<CreateGrowthTreatmentHandler> logger,
    [FromKeyedServices("ranch:growthTreatments")] IRepository<GrowthTreatment> repository)
    : IRequestHandler<CreateGrowthTreatmentCommand, CreateGrowthTreatmentResponse>
{
    public async Task<CreateGrowthTreatmentResponse> Handle(CreateGrowthTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var growthTreatment = GrowthTreatment.Create(
            request.Name,
            request.Description,
            request.DollarsPerHead
        );await repository.AddAsync(growthTreatment, cancellationToken);
        logger.LogInformation("growthTreatment created {GrowthTreatmentId}", growthTreatment.Id);
        return new CreateGrowthTreatmentResponse(growthTreatment.Id);
    }
}

