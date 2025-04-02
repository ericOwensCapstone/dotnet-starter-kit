using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Search.v1;
public sealed class SearchLifecycleProgramsHandler(
    [FromKeyedServices("ranch:lifecyclePrograms")] IReadRepository<LifecycleProgram> repository)
    : IRequestHandler<SearchLifecycleProgramsCommand, PagedList<LifecycleProgramResponse>>
{
    public async Task<PagedList<LifecycleProgramResponse>> Handle(SearchLifecycleProgramsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchLifecycleProgramSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<LifecycleProgramResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


