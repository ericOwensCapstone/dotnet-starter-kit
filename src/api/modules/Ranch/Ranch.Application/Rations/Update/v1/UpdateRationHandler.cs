using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Update.v1;
public sealed class UpdateRationHandler(
    ILogger<UpdateRationHandler> logger,
    [FromKeyedServices("ranch:rations")] IRepository<Ration> repository)
    : IRequestHandler<UpdateRationCommand, UpdateRationResponse>
{
    public async Task<UpdateRationResponse> Handle(UpdateRationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var ration = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = ration ?? throw new RationNotFoundException(request.Id);
        var updatedRation = ration.Update(
            request.Name,
            request.Description,
            request.DollarsPerPound
        );
        await repository.UpdateAsync(updatedRation, cancellationToken);
        logger.LogInformation("ration with id : {RationId} updated.", ration.Id);
        return new UpdateRationResponse(ration.Id);
    }
}

