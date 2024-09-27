using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.RationCatalog.Domain.Exceptions;
public sealed class RationNotFoundException : NotFoundException
{
    public RationNotFoundException(Guid id)
        : base($"ration with id {id} not found")
    {
    }
}

