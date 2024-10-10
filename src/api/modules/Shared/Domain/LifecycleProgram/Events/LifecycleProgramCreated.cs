using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Events;
public sealed record LifecycleProgramCreated : DomainEvent
{
    public LifecycleProgram? LifecycleProgram { get; set; }
}

