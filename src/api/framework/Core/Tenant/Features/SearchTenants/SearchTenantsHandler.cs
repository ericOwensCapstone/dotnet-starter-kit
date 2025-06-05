using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Core.Tenant.Dtos;
using MediatR;

namespace FSH.Framework.Core.Tenant.Features.SearchTenants;

public sealed class SearchTenantsHandler(ITenantService service) : IRequestHandler<SearchTenantsQuery, PagedList<TenantDetail>>
{
    public Task<PagedList<TenantDetail>> Handle(SearchTenantsQuery request, CancellationToken cancellationToken)
    {
        return service.SearchAsync(request.SearchTerm, request.PageNumber, request.PageSize, request.OrderBy);
    }
}