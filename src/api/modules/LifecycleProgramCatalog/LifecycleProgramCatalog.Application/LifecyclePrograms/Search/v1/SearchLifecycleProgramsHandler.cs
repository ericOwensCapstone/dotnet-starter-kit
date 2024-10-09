using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Search.v1;
public sealed class SearchLifecycleProgramsHandler(
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IReadRepository<LifecycleProgram> repository)
    : IRequestHandler<SearchLifecycleProgramsCommand, PagedList<LifecycleProgramResponse>>
{
    public async Task<PagedList<LifecycleProgramResponse>> Handle(SearchLifecycleProgramsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<LifecycleProgram, LifecycleProgramResponse>(request.filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<LifecycleProgramResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}


