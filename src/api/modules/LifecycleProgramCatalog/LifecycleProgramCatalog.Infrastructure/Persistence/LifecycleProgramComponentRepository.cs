using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Persistence;
public class LifecycleProgramComponentRepository<Tp> : LifecycleProgramCatalogRepository<Tp>, IComponentRepository<Tp>
    where Tp : class, IAggregateRoot
{
    private SharedDbContext _context;
    public LifecycleProgramComponentRepository(SharedDbContext context)
        : base(context)
    {
        _context = context;
    }

    async Task<Tn> IComponentRepository<Tp>.GetComponentByIdAsync<Tn>(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Tn>().FindAsync([id], cancellationToken: cancellationToken);
    }
}
