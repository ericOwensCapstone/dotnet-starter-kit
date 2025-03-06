using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.Rations;

namespace FSH.Starter.WebApi.Ranch.Domain.Rations.Events;
public sealed record RationUpdated : DomainEvent
{
    public Ration? Ration { get; set; }
}

