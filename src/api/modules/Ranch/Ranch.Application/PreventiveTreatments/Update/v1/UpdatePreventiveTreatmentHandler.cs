using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Update.v1;
public sealed class UpdatePreventiveTreatmentHandler(
    ILogger<UpdatePreventiveTreatmentHandler> logger,
    [FromKeyedServices("ranch:preventiveTreatments")] IRepository<PreventiveTreatment> repository)
    : IRequestHandler<UpdatePreventiveTreatmentCommand, UpdatePreventiveTreatmentResponse>
{
    public async Task<UpdatePreventiveTreatmentResponse> Handle(UpdatePreventiveTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var preventiveTreatment = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = preventiveTreatment ?? throw new PreventiveTreatmentNotFoundException(request.Id);
        
        var updatedPreventiveTreatment = preventiveTreatment.Update(
            request.Name,
            request.Description,
            request.DollarsPerHead
        );
        await repository.UpdateAsync(updatedPreventiveTreatment, cancellationToken);
        logger.LogInformation("preventiveTreatment with id : {PreventiveTreatmentId} updated.", preventiveTreatment.Id);
        return new UpdatePreventiveTreatmentResponse(preventiveTreatment.Id);
    }
}

