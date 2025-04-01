using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments.Events;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
public class GrowthTreatment : AuditableEntity, IAggregateRoot
{
            public string Name { get; private set; } = string.Empty; // MaxLength=99, Default="Sample GrowthTreatment", Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
            public string? Description { get; private set; } = string.Empty; // MaxLength=999, Default="Descriptive Description", Rule=NotEmpty().MinimumLength(2).MaximumLength(999)
            public decimal DollarsPerHead { get; private set; } = 0; // Default=1.10, Rule=GreaterThan(0)

    private GrowthTreatment() { }

    private GrowthTreatment(
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

        QueueDomainEvent(new GrowthTreatmentCreated {GrowthTreatment = this});
    }

    public static GrowthTreatment Create(
        string name,
        string? description,
        decimal dollarsPerHead
    )
    {
        var newId = Guid.NewGuid();
        return new GrowthTreatment(
            newId,
            name,
            description,
            dollarsPerHead
        );
    }

    public GrowthTreatment Update(
        string name,
        string? description,
        decimal dollarsPerHead
    )
    {
        Name = name;
        Description = description;
        DollarsPerHead = dollarsPerHead;
        QueueDomainEvent(new GrowthTreatmentUpdated { GrowthTreatment = this });
        return this;
    }

    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = !context.Set<LifecycleStage>().Any(ls => ls.GrowthTreatmentId == Id && ls.Deleted == null);
        string reason = canBeDeleted ? string.Empty : "Cannot soft delete GrowthTreatment because it is referenced by a non-soft-deleted LifecycleStage.";
        return (canBeDeleted, reason);
    }
}


