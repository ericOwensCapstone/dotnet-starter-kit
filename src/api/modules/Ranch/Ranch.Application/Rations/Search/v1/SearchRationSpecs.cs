using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.Rations;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Search.v1;
public class SearchRationSpecs : EntitiesByPaginationFilterSpec<Ration, RationResponse>
{
    public SearchRationSpecs(SearchRationsCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.Name, !command.HasOrderBy())
            .Where(p => p.Price >= command.MinimumRate!.Value, command.MinimumRate.HasValue)
            .Where(p => p.Price <= command.MaximumRate!.Value, command.MaximumRate.HasValue);
}

