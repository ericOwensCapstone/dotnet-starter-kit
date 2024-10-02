using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Get.v1;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Search.v1;
public sealed class SearchGrowthTreatmentsHandler(
    [FromKeyedServices("growthTreatmentcatalog:growthTreatments")] IReadRepository<GrowthTreatment> repository)
    : IRequestHandler<SearchGrowthTreatmentsCommand, PagedList<GrowthTreatmentResponse>>
{
    public async Task<PagedList<GrowthTreatmentResponse>> Handle(SearchGrowthTreatmentsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<GrowthTreatment, GrowthTreatmentResponse>(request.filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<GrowthTreatmentResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}


