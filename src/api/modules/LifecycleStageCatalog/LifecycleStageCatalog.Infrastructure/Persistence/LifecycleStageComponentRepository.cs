using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
public class LifecycleStageComponentRepository<Tp> : LifecycleStageCatalogRepository<Tp>, IComponentRepository<Tp>
    where Tp : class, IAggregateRoot
{
    private SharedDbContext _context;
    public LifecycleStageComponentRepository(SharedDbContext context)
        : base(context)
    {
        _context = context;
    }

    async Task<Tn> IComponentRepository<Tp>.GetComponentByIdAsync<Tn>(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Tn>().FindAsync([id], cancellationToken: cancellationToken);
    }

}
