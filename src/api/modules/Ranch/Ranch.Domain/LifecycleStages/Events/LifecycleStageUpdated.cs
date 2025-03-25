using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;

namespace FSH.Starter.WebApi.Ranch.Domain.LifecycleStages.Events;
public sealed record LifecycleStageUpdated : DomainEvent
{
    public LifecycleStage? LifecycleStage { get; set; }
}

