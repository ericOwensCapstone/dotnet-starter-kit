using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Search.v1;
public class SearchContractStatusSpecs : EntitiesByPaginationFilterSpec<ContractStatus, ContractStatusResponse>
{
    public SearchContractStatusSpecs(SearchContractStatusesCommand command)
        : base(command) =>
        Query
            //Start One Per Code//End One Per Code  
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

