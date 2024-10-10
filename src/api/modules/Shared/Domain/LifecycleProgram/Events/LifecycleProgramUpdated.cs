using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Events;
public sealed record LifecycleProgramUpdated : DomainEvent
{
    public LifecycleProgram? LifecycleProgram { get; set; }
}

