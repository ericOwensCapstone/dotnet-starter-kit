using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Create.v1;
public sealed class CreatePreventiveTreatmentHandler(
    ILogger<CreatePreventiveTreatmentHandler> logger,
    [FromKeyedServices("ranch:preventiveTreatments")] IRepository<PreventiveTreatment> repository)
    : IRequestHandler<CreatePreventiveTreatmentCommand, CreatePreventiveTreatmentResponse>
{
    public async Task<CreatePreventiveTreatmentResponse> Handle(CreatePreventiveTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var preventiveTreatment = PreventiveTreatment.Create(
            request.Name,
            request.Description,
            request.DollarsPerHead
        );await repository.AddAsync(preventiveTreatment, cancellationToken);
        logger.LogInformation("preventiveTreatment created {PreventiveTreatmentId}", preventiveTreatment.Id);
        return new CreatePreventiveTreatmentResponse(preventiveTreatment.Id);
    }
}

