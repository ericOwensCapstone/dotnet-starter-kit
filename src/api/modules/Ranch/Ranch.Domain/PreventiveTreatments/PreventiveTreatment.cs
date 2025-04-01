using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
public class PreventiveTreatment : AuditableEntity, IAggregateRoot
{
            public string Name { get; private set; } = string.Empty; // MaxLength=99, Default="Sample PreventiveTreatment", Rule=NotEmpty().MinimumLength(2).MaximumLength(98)
            public string? Description { get; private set; } = string.Empty; // MaxLength=999, Default="Descriptive Description", Rule=NotEmpty().MinimumLength(2).MaximumLength(998)
            public decimal DollarsPerHead { get; private set; } = 0; // Default=1.10, Rule=GreaterThan(0)

    private PreventiveTreatment() { }

    private PreventiveTreatment(
        Guid id,
        string name,
        string? description,
        decimal dollarsPerHead
    )
    {
        Id = id;
        Name = name;
        Description = description;
        DollarsPerHead = dollarsPerHead;

        QueueDomainEvent(new PreventiveTreatmentCreated {PreventiveTreatment = this});
    }

    public static PreventiveTreatment Create(
        string name,
        string? description,
        decimal dollarsPerHead
    )
    {
        var newId = Guid.NewGuid();
        return new PreventiveTreatment(
            newId,
            name,
            description,
            dollarsPerHead
        );
    }

    public PreventiveTreatment Update(
        string name,
        string? description,
        decimal dollarsPerHead
    )
    {
        Name = name;
        Description = description;
        DollarsPerHead = dollarsPerHead;
        QueueDomainEvent(new PreventiveTreatmentUpdated { PreventiveTreatment = this });
        return this;
    }

    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = !context.Set<LifecycleStage>().Any(ls => ls.PreventiveTreatmentId == Id && ls.Deleted == null);
        string reason = canBeDeleted ? string.Empty : "Cannot soft delete PreventiveTreatment because it is referenced by a non-soft-deleted LifecycleStage.";
        return (canBeDeleted, reason);
    }
}


