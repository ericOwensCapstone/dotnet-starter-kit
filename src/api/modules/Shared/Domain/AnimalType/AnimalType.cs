using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Events;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
public class AnimalType : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal DollarsPerPound { get; private set; }

    public static AnimalType Create(string name, string? description, decimal dollarsPerPound)
    {
        var animalType = new AnimalType();

        animalType.Name = name;
        animalType.Description = description;
        animalType.DollarsPerPound = dollarsPerPound;

        animalType.QueueDomainEvent(new AnimalTypeCreated() { AnimalType = animalType });

        return animalType;
    }

    public static AnimalType Create(Guid id, string name, string? description, decimal dollarsPerPound)
    {
        var animalType = new AnimalType
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerPound = dollarsPerPound
        };

        return animalType;
    }

    public AnimalType Update(string? name, string? description, decimal? dollarsPerPound)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (dollarsPerPound.HasValue && DollarsPerPound != dollarsPerPound) DollarsPerPound = dollarsPerPound.Value;

        this.QueueDomainEvent(new AnimalTypeUpdated() { AnimalType = this });
        return this;
    }

    public static AnimalType Update(Guid id, string name, string? description, decimal dollarsPerPound)
    {
        var animalType = new AnimalType
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerPound = dollarsPerPound
        };

        animalType.QueueDomainEvent(new AnimalTypeUpdated() { AnimalType = animalType });

        return animalType;
    }
}





