using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;
public sealed class GetLifecycleProgramHandler(
    [FromKeyedServices("ranch:lifecyclePrograms")] IReadRepository<LifecycleProgram> repository,
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
                var spec = new GetLifecycleProgramSpecs(request.Id);
                var lifecycleProgramItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (lifecycleProgramItem == null) throw new LifecycleProgramNotFoundException(request.Id);
                return lifecycleProgramItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

