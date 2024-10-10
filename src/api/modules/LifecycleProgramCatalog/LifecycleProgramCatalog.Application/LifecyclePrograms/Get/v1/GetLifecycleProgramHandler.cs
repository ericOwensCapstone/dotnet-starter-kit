using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
public sealed class GetLifecycleProgramHandler(
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IReadRepository<LifecycleProgram> repository,
    ICacheService cache)
    : IRequestHandler<GetLifecycleProgramRequest, LifecycleProgramResponse>
{
    public async Task<LifecycleProgramResponse> Handle(GetLifecycleProgramRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"lifecycleProgram:{request.Id}",
            async () =>
            {
                var lifecycleProgramItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (lifecycleProgramItem == null) throw new LifecycleProgramNotFoundException(request.Id);
                return new LifecycleProgramResponse(lifecycleProgramItem.Id, lifecycleProgramItem.Name, lifecycleProgramItem.Description, lifecycleProgramItem.Rating, null);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

