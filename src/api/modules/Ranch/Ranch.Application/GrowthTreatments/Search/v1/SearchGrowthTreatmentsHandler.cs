using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Search.v1;
public sealed class SearchGrowthTreatmentsHandler(
    [FromKeyedServices("ranch:growthTreatments")] IReadRepository<GrowthTreatment> repository)
    : IRequestHandler<SearchGrowthTreatmentsCommand, PagedList<GrowthTreatmentResponse>>
{
    public async Task<PagedList<GrowthTreatmentResponse>> Handle(SearchGrowthTreatmentsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchGrowthTreatmentSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<GrowthTreatmentResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


