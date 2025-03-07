using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;
public sealed class GetGrowthTreatmentHandler(
    [FromKeyedServices("ranch:growthTreatments")] IReadRepository<GrowthTreatment> repository,
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
                var spec = new GetGrowthTreatmentSpecs(request.Id);
                var growthTreatmentItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (growthTreatmentItem == null) throw new GrowthTreatmentNotFoundException(request.Id);
                return growthTreatmentItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

