using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Exceptions;
public sealed class LifecycleStageNotFoundException : NotFoundException
{
    public LifecycleStageNotFoundException(Guid id)
        : base($"lifecycleStage with id {id} not found")
    {
    }
}

