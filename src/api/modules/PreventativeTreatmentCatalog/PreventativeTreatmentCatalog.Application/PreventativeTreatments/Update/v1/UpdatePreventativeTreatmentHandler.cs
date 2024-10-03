using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Update.v1;
public sealed class UpdatePreventativeTreatmentHandler(
    ILogger<UpdatePreventativeTreatmentHandler> logger,
    [FromKeyedServices("preventativeTreatmentcatalog:preventativeTreatments")] IRepository<PreventativeTreatment> repository)
    : IRequestHandler<UpdatePreventativeTreatmentCommand, UpdatePreventativeTreatmentResponse>
{
    public async Task<UpdatePreventativeTreatmentResponse> Handle(UpdatePreventativeTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var preventativeTreatment = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = preventativeTreatment ?? throw new PreventativeTreatmentNotFoundException(request.Id);
        var updatedPreventativeTreatment = preventativeTreatment.Update(request.Name, request.Description, request.DollarsPerHead);
        await repository.UpdateAsync(updatedPreventativeTreatment, cancellationToken);
        logger.LogInformation("preventativeTreatment with id : {PreventativeTreatmentId} updated.", preventativeTreatment.Id);
        return new UpdatePreventativeTreatmentResponse(preventativeTreatment.Id);
    }
}

