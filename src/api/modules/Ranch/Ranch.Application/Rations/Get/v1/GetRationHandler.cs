using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.Rations.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.Rations;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
public sealed class GetRationHandler(
    [FromKeyedServices("ranch:rations")] IReadRepository<Ration> repository,
    ICacheService cache)
    : IRequestHandler<GetRationRequest, RationResponse>
{
    public async Task<RationResponse> Handle(GetRationRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"ration:{request.Id}",
            async () =>
            {
                var spec = new GetRationSpecs(request.Id);
                var rationItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (rationItem == null) throw new RationNotFoundException(request.Id);
                return rationItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

