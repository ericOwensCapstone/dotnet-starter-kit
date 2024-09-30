using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.RationCatalog.Domain.Events;

namespace FSH.Starter.WebApi.RationCatalog.Domain;
public class Ration : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal DollarsPerHeadPerDay { get; private set; }

    public static Ration Create(string name, string? description, decimal dollarsPerHeadPerDay)
    {
        var ration = new Ration();

        ration.Name = name;
        ration.Description = description;
        ration.DollarsPerHeadPerDay = dollarsPerHeadPerDay;

        ration.QueueDomainEvent(new RationCreated() { Ration = ration });

        return ration;
    }

    public Ration Update(string? name, string? description, decimal? dollarsPerHeadPerDay)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (dollarsPerHeadPerDay.HasValue && DollarsPerHeadPerDay != dollarsPerHeadPerDay) DollarsPerHeadPerDay = dollarsPerHeadPerDay.Value;

        this.QueueDomainEvent(new RationUpdated() { Ration = this });
        return this;
    }

    public static Ration Update(Guid id, string name, string? description, decimal dollarsPerHeadPerDay)
    {
        var ration = new Ration
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerHeadPerDay = dollarsPerHeadPerDay
        };

        ration.QueueDomainEvent(new RationUpdated() { Ration = ration });

        return ration;
    }
}

