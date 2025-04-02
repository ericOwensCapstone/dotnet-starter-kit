using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Search.v1;
public class SearchLifecycleProgramSpecs : EntitiesByPaginationFilterSpec<LifecycleProgram, LifecycleProgramResponse>
{
    public SearchLifecycleProgramSpecs(SearchLifecycleProgramsCommand command)
        : base(command) =>
        Query
            // Includes are implemented in context model builder
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

