using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Create.v1;
public sealed class CreatePreventativeTreatmentHandler(
    ILogger<CreatePreventativeTreatmentHandler> logger,
    [FromKeyedServices("preventativeTreatmentcatalog:preventativeTreatments")] IRepository<PreventativeTreatment> repository)
    : IRequestHandler<CreatePreventativeTreatmentCommand, CreatePreventativeTreatmentResponse>
{
    public async Task<CreatePreventativeTreatmentResponse> Handle(CreatePreventativeTreatmentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var preventativeTreatment = PreventativeTreatment.Create(request.Name!, request.Description, request.DollarsPerHead);
        await repository.AddAsync(preventativeTreatment, cancellationToken);
        logger.LogInformation("preventativeTreatment created {PreventativeTreatmentId}", preventativeTreatment.Id);
        return new CreatePreventativeTreatmentResponse(preventativeTreatment.Id);
    }
}

