using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.Rations.Events;

namespace FSH.Starter.WebApi.Ranch.Domain.Rations;
public class Ration : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty; // MaxLength=99, Default="Sample Ration", Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // MaxLength=999, Default="Ration Description", Rule=NotEmpty().MinimumLength(2).MaximumLength(999)
    public decimal DollarsPerPound { get; private set; } = 0; // Default=0.10, Rule=GreaterThan(0)

    private Ration() { }

    private Ration(
        Guid id,
        string name,
        string? description,
        decimal dollarsPerPound
    )
    {
        Id = id;
        Name = name;
        Description = description;
        DollarsPerPound = dollarsPerPound;

        QueueDomainEvent(new RationCreated {Ration = this});
    }

    public static Ration Create(
        string name,
        string? description,
        decimal dollarsPerPound
    )
    {
        return new Ration(
            Guid.NewGuid(),
            name,
            description,
            dollarsPerPound
        );
    }

    public Ration Update(
        string name,
        string? description,
        decimal dollarsPerPound
    )
    {
        Name = name;
        Description = description;
        DollarsPerPound = dollarsPerPound;
        QueueDomainEvent(new RationUpdated { Ration = this });
        return this;
    }
}


