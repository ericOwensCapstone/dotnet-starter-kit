using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.Rations.Search.v1;
public sealed class SearchRationsHandler(
    [FromKeyedServices("ranch:rations")] IReadRepository<Ration> repository)
    : IRequestHandler<SearchRationsCommand, PagedList<RationResponse>>
{
    public async Task<PagedList<RationResponse>> Handle(SearchRationsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchRationSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<RationResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


