using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.RationCatalog.Domain.Events;
public sealed record RationUpdated : DomainEvent
{
    public Ration? Ration { get; set; }
}

