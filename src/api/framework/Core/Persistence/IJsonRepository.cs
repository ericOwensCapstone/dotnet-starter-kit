
using FSH.Framework.Core.Domain.Contracts;

namespace FSH.Framework.Core.Persistence;
public interface IJsonRepository<T> : IRepository<T>
    where T : class, IAggregateRoot
{
    

}
