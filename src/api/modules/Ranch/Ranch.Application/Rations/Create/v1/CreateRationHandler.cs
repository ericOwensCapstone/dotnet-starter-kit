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
        //Start One Per Code
        //End One Per Code
        var ration = Ration.Create(
            request.Name,
            request.Description,
            request.DollarsPerPound
        );
        await repository.AddAsync(ration, cancellationToken);
        logger.LogInformation("ration created {RationId}", ration.Id);
        return new CreateRationResponse(ration.Id);
    }
}

