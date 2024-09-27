using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.RationCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Update.v1;
public sealed class UpdateRationHandler(
    ILogger<UpdateRationHandler> logger,
    [FromKeyedServices("rationcatalog:rations")] IRepository<Ration> repository)
    : IRequestHandler<UpdateRationCommand, UpdateRationResponse>
{
    public async Task<UpdateRationResponse> Handle(UpdateRationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var ration = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = ration ?? throw new RationNotFoundException(request.Id);
        var updatedRation = ration.Update(request.Name, request.Description, request.Price);
        await repository.UpdateAsync(updatedRation, cancellationToken);
        logger.LogInformation("ration with id : {RationId} updated.", ration.Id);
        return new UpdateRationResponse(ration.Id);
    }
}

