using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Delete.v1;
public sealed class DeletePreventiveTreatmentHandler(
    ILogger<DeletePreventiveTreatmentHandler> logger,
    [FromKeyedServices("ranch:preventiveTreatments")] IRepository<PreventiveTreatment> repository)
    : IRequestHandler<DeletePreventiveTreatmentCommand>
{
    public async Task Handle(DeletePreventiveTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var preventiveTreatment = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = preventiveTreatment ?? throw new PreventiveTreatmentNotFoundException(request.Id);
        await repository.DeleteAsync(preventiveTreatment, cancellationToken);
        logger.LogInformation("preventiveTreatment with id : {PreventiveTreatmentId} deleted", preventiveTreatment.Id);
    }
}

