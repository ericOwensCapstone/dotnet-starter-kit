using Ardalis.Specification;
using FSH.Framework.Core.Domain.Contracts;

namespace FSH.Framework.Core.Persistence;
public interface IRepository<T> : IRepositoryBase<T>
    where T : class, IAggregateRoot
{
    //Task GetAndAttach<Tp>(Guid id, CancellationToken cancellationToken) where Tp : class;
}

public interface IReadRepository<T> : IReadRepositoryBase<T>
    where T : class, IAggregateRoot
{
}
