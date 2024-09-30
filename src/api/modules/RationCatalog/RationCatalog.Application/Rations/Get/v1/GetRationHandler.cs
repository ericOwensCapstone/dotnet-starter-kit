using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.RationCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Get.v1;
public sealed class GetRationHandler(
    [FromKeyedServices("rationcatalog:rations")] IReadRepository<Ration> repository,
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
                var rationItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (rationItem == null) throw new RationNotFoundException(request.Id);
                return new RationResponse(rationItem.Id, rationItem.Name, rationItem.Description, rationItem.DollarsPerHeadPerDay);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

