using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
public sealed class GetPreventativeTreatmentHandler(
    [FromKeyedServices("preventativeTreatmentcatalog:preventativeTreatments")] IReadRepository<PreventativeTreatment> repository,
    ICacheService cache)
    : IRequestHandler<GetPreventativeTreatmentRequest, PreventativeTreatmentResponse>
{
    public async Task<PreventativeTreatmentResponse> Handle(GetPreventativeTreatmentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"preventativeTreatment:{request.Id}",
            async () =>
            {
                var preventativeTreatmentItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (preventativeTreatmentItem == null) throw new PreventativeTreatmentNotFoundException(request.Id);
                return new PreventativeTreatmentResponse(preventativeTreatmentItem.Id, preventativeTreatmentItem.Name, preventativeTreatmentItem.Description, preventativeTreatmentItem.DollarsPerHead);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

