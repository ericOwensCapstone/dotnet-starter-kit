using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Update.v1;
public sealed class UpdateGrowthTreatmentHandler(
    ILogger<UpdateGrowthTreatmentHandler> logger,
    [FromKeyedServices("growthTreatmentcatalog:growthTreatments")] IRepository<GrowthTreatment> repository)
    : IRequestHandler<UpdateGrowthTreatmentCommand, UpdateGrowthTreatmentResponse>
{
    public async Task<UpdateGrowthTreatmentResponse> Handle(UpdateGrowthTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var growthTreatment = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = growthTreatment ?? throw new GrowthTreatmentNotFoundException(request.Id);
        var updatedGrowthTreatment = growthTreatment.Update(request.Name, request.Description, request.DollarsPerHead);
        await repository.UpdateAsync(updatedGrowthTreatment, cancellationToken);
        logger.LogInformation("growthTreatment with id : {GrowthTreatmentId} updated.", growthTreatment.Id);
        return new UpdateGrowthTreatmentResponse(growthTreatment.Id);
    }
}

