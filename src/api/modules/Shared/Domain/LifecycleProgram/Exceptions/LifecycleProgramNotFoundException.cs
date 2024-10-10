using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Exceptions;
public sealed class LifecycleProgramNotFoundException : NotFoundException
{
    public LifecycleProgramNotFoundException(Guid id)
        : base($"lifecycleProgram with id {id} not found")
    {
    }
}

