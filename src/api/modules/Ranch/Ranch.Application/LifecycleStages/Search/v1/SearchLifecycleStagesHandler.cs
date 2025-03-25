using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Search.v1;
public sealed class SearchLifecycleStagesHandler(
    [FromKeyedServices("ranch:lifecycleStages")] IReadRepository<LifecycleStage> repository)
    : IRequestHandler<SearchLifecycleStagesCommand, PagedList<LifecycleStageResponse>>
{
    public async Task<PagedList<LifecycleStageResponse>> Handle(SearchLifecycleStagesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchLifecycleStageSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<LifecycleStageResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


