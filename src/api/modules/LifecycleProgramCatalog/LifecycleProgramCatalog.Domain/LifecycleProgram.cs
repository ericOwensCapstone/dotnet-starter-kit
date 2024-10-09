using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Events;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
public class LifecycleProgram : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Rating { get; private set; }

    public static LifecycleProgram Create(string name, string? description, decimal rating)
    {
        var lifecycleProgram = new LifecycleProgram();

        lifecycleProgram.Name = name;
        lifecycleProgram.Description = description;
        lifecycleProgram.Rating = rating;

        lifecycleProgram.QueueDomainEvent(new LifecycleProgramCreated() { LifecycleProgram = lifecycleProgram });

        return lifecycleProgram;
    }

    public LifecycleProgram Update(string? name, string? description, decimal? rating)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (rating.HasValue && Rating != rating) Rating = rating.Value;

        this.QueueDomainEvent(new LifecycleProgramUpdated() { LifecycleProgram = this });
        return this;
    }

    public static LifecycleProgram Update(Guid id, string name, string? description, decimal rating)
    {
        var lifecycleProgram = new LifecycleProgram
        {
            Id = id,
            Name = name,
            Description = description,
            Rating = rating
        };

        lifecycleProgram.QueueDomainEvent(new LifecycleProgramUpdated() { LifecycleProgram = lifecycleProgram });

        return lifecycleProgram;
    }
}

