using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Delete.v1;
public sealed class DeletePreventativeTreatmentHandler(
    ILogger<DeletePreventativeTreatmentHandler> logger,
    [FromKeyedServices("preventativeTreatmentcatalog:preventativeTreatments")] IRepository<PreventativeTreatment> repository)
    : IRequestHandler<DeletePreventativeTreatmentCommand>
{
    public async Task Handle(DeletePreventativeTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var preventativeTreatment = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = preventativeTreatment ?? throw new PreventativeTreatmentNotFoundException(request.Id);
        await repository.DeleteAsync(preventativeTreatment, cancellationToken);
        logger.LogInformation("preventativeTreatment with id : {PreventativeTreatmentId} deleted", preventativeTreatment.Id);
    }
}

