using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

namespace FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms.Events;
public sealed record LifecycleProgramUpdated : DomainEvent
{
    public LifecycleProgram? LifecycleProgram { get; set; }
}

