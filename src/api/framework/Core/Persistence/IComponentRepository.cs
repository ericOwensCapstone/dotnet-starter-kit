using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;

namespace FSH.Framework.Core.Persistence;
public interface IComponentRepository<Tp> : IRepository<Tp>
    where Tp : class, IAggregateRoot
{
    Task<Tn> GetComponentByIdAsync<Tn>(Guid id, CancellationToken cancellationToken = default) where Tn : class, IAggregateRoot;

}
