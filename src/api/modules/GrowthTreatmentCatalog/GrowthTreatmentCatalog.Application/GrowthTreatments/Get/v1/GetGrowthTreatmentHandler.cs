using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Get.v1;
public sealed class GetGrowthTreatmentHandler(
    [FromKeyedServices("growthTreatmentcatalog:growthTreatments")] IReadRepository<GrowthTreatment> repository,
    ICacheService cache)
    : IRequestHandler<GetGrowthTreatmentRequest, GrowthTreatmentResponse>
{
    public async Task<GrowthTreatmentResponse> Handle(GetGrowthTreatmentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"growthTreatment:{request.Id}",
            async () =>
            {
                var growthTreatmentItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (growthTreatmentItem == null) throw new GrowthTreatmentNotFoundException(request.Id);
                return new GrowthTreatmentResponse(growthTreatmentItem.Id, growthTreatmentItem.Name, growthTreatmentItem.Description, growthTreatmentItem.DollarsPerHead);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

