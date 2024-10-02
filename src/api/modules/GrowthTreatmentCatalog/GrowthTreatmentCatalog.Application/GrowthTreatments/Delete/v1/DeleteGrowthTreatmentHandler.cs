using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Delete.v1;
public sealed class DeleteGrowthTreatmentHandler(
    ILogger<DeleteGrowthTreatmentHandler> logger,
    [FromKeyedServices("growthTreatmentcatalog:growthTreatments")] IRepository<GrowthTreatment> repository)
    : IRequestHandler<DeleteGrowthTreatmentCommand>
{
    public async Task Handle(DeleteGrowthTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var growthTreatment = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = growthTreatment ?? throw new GrowthTreatmentNotFoundException(request.Id);
        await repository.DeleteAsync(growthTreatment, cancellationToken);
        logger.LogInformation("growthTreatment with id : {GrowthTreatmentId} deleted", growthTreatment.Id);
    }
}

