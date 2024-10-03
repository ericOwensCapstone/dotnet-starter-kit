using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Events;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
public class LifecycleStage : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Rating { get; private set; }

    public static LifecycleStage Create(string name, string? description, decimal rating)
    {
        var lifecycleStage = new LifecycleStage();

        lifecycleStage.Name = name;
        lifecycleStage.Description = description;
        lifecycleStage.Rating = rating;

        lifecycleStage.QueueDomainEvent(new LifecycleStageCreated() { LifecycleStage = lifecycleStage });

        return lifecycleStage;
    }

    public LifecycleStage Update(string? name, string? description, decimal? rating)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (rating.HasValue && Rating != rating) Rating = rating.Value;

        this.QueueDomainEvent(new LifecycleStageUpdated() { LifecycleStage = this });
        return this;
    }

    public static LifecycleStage Update(Guid id, string name, string? description, decimal rating)
    {
        var lifecycleStage = new LifecycleStage
        {
            Id = id,
            Name = name,
            Description = description,
            Rating = rating
        };

        lifecycleStage.QueueDomainEvent(new LifecycleStageUpdated() { LifecycleStage = lifecycleStage });

        return lifecycleStage;
    }
}

