using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.RationCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Delete.v1;
public sealed class DeleteRationHandler(
    ILogger<DeleteRationHandler> logger,
    [FromKeyedServices("rationcatalog:rations")] IRepository<Ration> repository)
    : IRequestHandler<DeleteRationCommand>
{
    public async Task Handle(DeleteRationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var ration = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = ration ?? throw new RationNotFoundException(request.Id);
        await repository.DeleteAsync(ration, cancellationToken);
        logger.LogInformation("ration with id : {RationId} deleted", ration.Id);
    }
}

