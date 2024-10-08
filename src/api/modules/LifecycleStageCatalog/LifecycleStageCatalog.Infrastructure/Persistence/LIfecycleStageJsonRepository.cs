using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;
using Mapster;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
internal class LifecycleStageCatalogJsonRepository<T> : LifecycleStageCatalogRepository<T>, IJsonRepository<T>
    where T : class, IAggregateRoot
{
    private readonly LifecycleStageCatalogDbContext _context;
    public LifecycleStageCatalogJsonRepository(LifecycleStageCatalogDbContext context)
        : base(context)
    {
        _context = context;
    }






}
