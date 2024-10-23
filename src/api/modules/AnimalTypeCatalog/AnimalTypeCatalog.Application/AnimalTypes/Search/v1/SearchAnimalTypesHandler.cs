using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Search.v1;
public sealed class SearchAnimalTypesHandler(
    [FromKeyedServices("animalTypecatalog:animalTypes")] IReadRepository<AnimalType> repository)
    : IRequestHandler<SearchAnimalTypesCommand, PagedList<AnimalTypeResponse>>
{
    public async Task<PagedList<AnimalTypeResponse>> Handle(SearchAnimalTypesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<AnimalType, AnimalTypeResponse>(request.filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<AnimalTypeResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}



