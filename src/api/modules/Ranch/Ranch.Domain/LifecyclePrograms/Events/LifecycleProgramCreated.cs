using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

namespace FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms.Events;
public sealed record LifecycleProgramCreated : DomainEvent
{
    public LifecycleProgram? LifecycleProgram { get; set; }
}

