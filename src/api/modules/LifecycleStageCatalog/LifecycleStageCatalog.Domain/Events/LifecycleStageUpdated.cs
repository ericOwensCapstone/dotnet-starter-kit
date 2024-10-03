using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Events;
public sealed record LifecycleStageUpdated : DomainEvent
{
    public LifecycleStage? LifecycleStage { get; set; }
}

