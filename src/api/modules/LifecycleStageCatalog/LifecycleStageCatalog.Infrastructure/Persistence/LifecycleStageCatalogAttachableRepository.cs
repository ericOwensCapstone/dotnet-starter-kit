using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;
using Mapster;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
internal sealed class LifecycleStageCatalogAttachableRepository<T> : LifecycleStageCatalogRepository<T>, IAttachableRepository<T>
    where T : class, IAggregateRoot
{
    private readonly LifecycleStageCatalogDbContext _context;
    public LifecycleStageCatalogAttachableRepository(LifecycleStageCatalogDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task GetAndAttach<Tp>(Guid id, CancellationToken cancellationToken) where Tp : class
    {
        var temp = await _context.Set<Tp>().FindAsync(new object[] { id }, cancellationToken);
        if(temp != null)
        {
            _context.Attach<Tp>(temp);
        }
    }


}

