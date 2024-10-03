using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.RationCatalog.Domain.Events;

namespace FSH.Starter.WebApi.RationCatalog.Domain;
public class Ration : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal DollarsPerPound { get; private set; }

    public static Ration Create(string name, string? description, decimal dollarsPerPound)
    {
        var ration = new Ration();

        ration.Name = name;
        ration.Description = description;
        ration.DollarsPerPound = dollarsPerPound;

        ration.QueueDomainEvent(new RationCreated() { Ration = ration });

        return ration;
    }

    public Ration Update(string? name, string? description, decimal? dollarsPerPound)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (dollarsPerPound.HasValue && DollarsPerPound != dollarsPerPound) DollarsPerPound = dollarsPerPound.Value;

        this.QueueDomainEvent(new RationUpdated() { Ration = this });
        return this;
    }

    public static Ration Update(Guid id, string name, string? description, decimal dollarsPerPound)
    {
        var ration = new Ration
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerPound = dollarsPerPound
        };

        ration.QueueDomainEvent(new RationUpdated() { Ration = ration });

        return ration;
    }
}

