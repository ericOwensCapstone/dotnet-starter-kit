using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Create.v1;
public sealed class CreateLifecycleProgramHandler(
    ILogger<CreateLifecycleProgramHandler> logger,
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IRepository<LifecycleProgram> repository)
    : IRequestHandler<CreateLifecycleProgramCommand, CreateLifecycleProgramResponse>
{
    public async Task<CreateLifecycleProgramResponse> Handle(CreateLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgram = LifecycleProgram.Create(request.Name!, request.Description, request.Rating);
        await repository.AddAsync(lifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram created {LifecycleProgramId}", lifecycleProgram.Id);
        return new CreateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

