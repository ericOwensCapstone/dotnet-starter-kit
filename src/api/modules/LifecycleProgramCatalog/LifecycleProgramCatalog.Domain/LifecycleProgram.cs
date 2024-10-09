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

    public List<LifecycleStage>? LifecycleStages { get; private set; }

    public static LifecycleProgram Create(string name, string? description, decimal rating, List<LifecycleStage> lifecycleStages)
    {
        var lifecycleProgram = new LifecycleProgram();

        lifecycleProgram.Name = name;
        lifecycleProgram.Description = description;
        lifecycleProgram.LifecycleStages = lifecycleStages;
        lifecycleProgram.Rating = rating;

        lifecycleProgram.QueueDomainEvent(new LifecycleProgramCreated() { LifecycleProgram = lifecycleProgram });

        return lifecycleProgram;
    }

    public LifecycleProgram Update(string? name, string? description, decimal? rating, List<LifecycleStage>? lifecycleStages)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (rating.HasValue && Rating != rating) Rating = rating.Value;

        //TODO consider adding logic to see if updates are needed here
        if (lifecycleStages is not null)
        {
            LifecycleStages = lifecycleStages;
        }

        this.QueueDomainEvent(new LifecycleProgramUpdated() { LifecycleProgram = this });
        return this;
    }

    public static LifecycleProgram Update(Guid id, string name, string? description, decimal rating, List<LifecycleStage>? lifecycleStages)
    {
        var lifecycleProgram = new LifecycleProgram
        {
            Id = id,
            Name = name,
            Description = description,
            Rating = rating,
            LifecycleStages = lifecycleStages
        };

        lifecycleProgram.QueueDomainEvent(new LifecycleProgramUpdated() { LifecycleProgram = lifecycleProgram });

        return lifecycleProgram;
    }
}

