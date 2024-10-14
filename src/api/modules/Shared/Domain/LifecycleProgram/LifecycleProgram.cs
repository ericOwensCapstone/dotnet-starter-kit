using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Events;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
public class LifecycleProgram : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Rating { get; private set; }
    public List<LifecycleProgramStage>? LifecycleProgramStages { get; private set; } = new();

    public static LifecycleProgram Create(string name, string? description, decimal rating, List<LifecycleProgramStage> lifecycleProgramStages)
    {
        var lifecycleProgram = new LifecycleProgram();

        lifecycleProgram.Name = name;
        lifecycleProgram.Description = description;
        foreach(var lifecycleProgramStage in lifecycleProgramStages)
        {
            lifecycleProgramStage.LifecycleProgram = lifecycleProgram;
        }
        lifecycleProgram.LifecycleProgramStages = lifecycleProgramStages;
        lifecycleProgram.Rating = rating;

        lifecycleProgram.QueueDomainEvent(new LifecycleProgramCreated() { LifecycleProgram = lifecycleProgram });

        return lifecycleProgram;
    }

    public LifecycleProgram Update(string? name, string? description, decimal? rating, List<LifecycleProgramStage>? lifecycleProgramStages)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (rating.HasValue && Rating != rating) Rating = rating.Value;

        if (lifecycleProgramStages is not null)
        {
            LifecycleProgramStages = lifecycleProgramStages;
        }

        this.QueueDomainEvent(new LifecycleProgramUpdated() { LifecycleProgram = this });
        return this;
    }

    public static LifecycleProgram Update(Guid id, string name, string? description, decimal rating, List<LifecycleProgramStage>? lifecycleProgramStages)
    {
        var lifecycleProgram = new LifecycleProgram
        {
            Id = id,
            Name = name,
            Description = description,
            Rating = rating,
            LifecycleProgramStages = lifecycleProgramStages
        };

        lifecycleProgram.QueueDomainEvent(new LifecycleProgramUpdated() { LifecycleProgram = lifecycleProgram });

        return lifecycleProgram;
    }
}

