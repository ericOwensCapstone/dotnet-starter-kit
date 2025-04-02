using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
public class LifecycleProgram : AuditableEntity, IAggregateRoot
{

    public string Name { get; private set; } = string.Empty; // MaxLength=100, Default="Sample LifecycleProgram", Rule=NotEmpty().MinimumLength(2).MaximumLength(100)
    public string? Description { get; private set; } = string.Empty; // MaxLength=1000, Default="Descriptive Description", Rule=NotEmpty().MinimumLength(2).MaximumLength(1000)
    public List<LifecycleProgramLifecycleStage> LifecycleProgramLifecycleStages { get; private set; } = new(); // Rule=NotEmpty()

    private LifecycleProgram() { }

    private LifecycleProgram(
        Guid id,
        string name,
        string? description,
        List<LifecycleProgramLifecycleStage> lifecycleProgramLifecycleStages
    )
    {
        Id = id;
        Name = name;
        Description = description;
        LifecycleProgramLifecycleStages = lifecycleProgramLifecycleStages;

        QueueDomainEvent(new LifecycleProgramCreated {LifecycleProgram = this});
    }

    public static LifecycleProgram Create(
        string name,
        string? description,
        List<LifecycleProgramLifecycleStage> lifecycleProgramLifecycleStages
    )
    {
        var newId = Guid.NewGuid();
        foreach(var i in lifecycleProgramLifecycleStages)
        {
            i.LifecycleProgramId = newId;
        }
        return new LifecycleProgram(
            newId,
            name,
            description,
            lifecycleProgramLifecycleStages
        );
    }

    public LifecycleProgram Update(
        string name,
        string? description,
        List<LifecycleProgramLifecycleStage> lifecycleProgramLifecycleStages
    )
    {
        Name = name;
        Description = description;
        LifecycleProgramLifecycleStages = lifecycleProgramLifecycleStages;
        QueueDomainEvent(new LifecycleProgramUpdated { LifecycleProgram = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


