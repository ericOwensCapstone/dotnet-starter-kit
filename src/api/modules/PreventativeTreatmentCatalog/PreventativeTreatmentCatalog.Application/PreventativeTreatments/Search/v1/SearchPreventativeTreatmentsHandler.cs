using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Search.v1;
public sealed class SearchPreventativeTreatmentsHandler(
    [FromKeyedServices("preventativeTreatmentcatalog:preventativeTreatments")] IReadRepository<PreventativeTreatment> repository)
    : IRequestHandler<SearchPreventativeTreatmentsCommand, PagedList<PreventativeTreatmentResponse>>
{
    public async Task<PagedList<PreventativeTreatmentResponse>> Handle(SearchPreventativeTreatmentsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<PreventativeTreatment, PreventativeTreatmentResponse>(request.filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<PreventativeTreatmentResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}


