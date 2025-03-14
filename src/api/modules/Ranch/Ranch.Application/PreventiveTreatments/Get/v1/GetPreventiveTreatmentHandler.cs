using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
public sealed class GetPreventiveTreatmentHandler(
    [FromKeyedServices("ranch:preventiveTreatments")] IReadRepository<PreventiveTreatment> repository,
    ICacheService cache)
    : IRequestHandler<GetPreventiveTreatmentRequest, PreventiveTreatmentResponse>
{
    public async Task<PreventiveTreatmentResponse> Handle(GetPreventiveTreatmentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"preventiveTreatment:{request.Id}",
            async () =>
            {
                var spec = new GetPreventiveTreatmentSpecs(request.Id);
                var preventiveTreatmentItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (preventiveTreatmentItem == null) throw new PreventiveTreatmentNotFoundException(request.Id);
                return preventiveTreatmentItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

