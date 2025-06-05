using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Tenant.Dtos;
using MediatR;

namespace FSH.Framework.Core.Tenant.Features.SearchTenants;

public class SearchTenantsQuery : PaginationFilter, IRequest<PagedList<TenantDetail>>
{
    public string? SearchTerm { get; set; }
    public string OrderBy { get; set; } = "name";
}