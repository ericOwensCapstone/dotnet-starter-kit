using FSH.Framework.Core.Domain.Contracts;

namespace FSH.Framework.Core.Persistence;
public interface IAttachableRepository<T> : IRepository<T>
    where T : class, IAggregateRoot
{
    Task Attach<Tp>(Guid id, CancellationToken cancellationToken) where Tp : class;

    Task<Tp> Get<Tp>(Guid id, CancellationToken cancellationToken) where Tp : class;

}

