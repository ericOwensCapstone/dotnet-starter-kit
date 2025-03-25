using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Ranch.Domain.LifecycleStages.Exceptions;
public sealed class LifecycleStageNotFoundException : NotFoundException
{
    public LifecycleStageNotFoundException(Guid id)
        : base($"lifecycleStage with id {id} not found")
    {
    }
}

