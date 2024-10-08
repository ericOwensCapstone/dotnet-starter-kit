using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Search.v1;
public sealed class SearchLifecycleStagesHandler(
    [FromKeyedServices("lifecycleStagecatalog:lifecycleStages")] IReadRepository<LifecycleStage> repository)
    : IRequestHandler<SearchLifecycleStagesCommand, PagedList<LifecycleStageResponse>>
{
    public async Task<PagedList<LifecycleStageResponse>> Handle(SearchLifecycleStagesCommand request, CancellationToken cancellationToken)
    {
        //TODO
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<LifecycleStage, LifecycleStageResponse>(request.filter);
        List<LifecycleStageResponse> items = null;
        try
        {
            items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var wd = 40;
            throw;
        }

        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<LifecycleStageResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}


