using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;

namespace FSH.Framework.Infrastructure.Identity.Persistence;

public class IdentityRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public IdentityRepository(IdentityDbContext dbContext)
        : base(dbContext)
    {
    }
}