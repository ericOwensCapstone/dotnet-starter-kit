using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Search.v1;
public sealed class SearchPreventiveTreatmentsHandler(
    [FromKeyedServices("ranch:preventiveTreatments")] IReadRepository<PreventiveTreatment> repository)
    : IRequestHandler<SearchPreventiveTreatmentsCommand, PagedList<PreventiveTreatmentResponse>>
{
    public async Task<PagedList<PreventiveTreatmentResponse>> Handle(SearchPreventiveTreatmentsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchPreventiveTreatmentSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<PreventiveTreatmentResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


