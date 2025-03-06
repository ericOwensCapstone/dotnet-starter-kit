using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Create.v1;
public sealed class CreateRationHandler(
    ILogger<CreateRationHandler> logger,
    [FromKeyedServices("ranch:rations")] IRepository<Ration> repository)
    : IRequestHandler<CreateRationCommand, CreateRationResponse>
{
    public async Task<CreateRationResponse> Handle(CreateRationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var ration = Ration.Create(request.Name!, request.Description, request.Price);
        await repository.AddAsync(ration, cancellationToken);
        logger.LogInformation("ration created {RationId}", ration.Id);
        return new CreateRationResponse(ration.Id);
    }
}

