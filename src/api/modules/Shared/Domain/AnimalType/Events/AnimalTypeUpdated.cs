using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Events;
public sealed record AnimalTypeUpdated : DomainEvent
{
    public AnimalType? AnimalType { get; set; }
}


