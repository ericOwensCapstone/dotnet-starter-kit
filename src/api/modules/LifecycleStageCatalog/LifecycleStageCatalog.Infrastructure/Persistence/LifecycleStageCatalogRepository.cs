using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
public class LifecycleStageCatalogRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    //private LifecycleStageCatalogDbContext _context;
    public LifecycleStageCatalogRepository(SharedDbContext context)
        : base(context)
    {
        //_context = context;
    }

    // We override the default behavior when mapping to a dto.
    // We're using Mapster's ProjectToType here to immediately map the result from the database.
    // This is only done when no Selector is defined, so regular specifications with a selector also still work.
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();
}

