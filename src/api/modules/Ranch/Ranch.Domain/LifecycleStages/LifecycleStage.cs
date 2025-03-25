using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages.Events;

namespace FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
public class LifecycleStage : AuditableEntity, IAggregateRoot
{

    public string Name { get; private set; } = string.Empty; // MaxLength=99, Default="Sample LifecycleStage", Rule=NotEmpty().MinimumLength(2).MaximumLength(100)
    public string? Description { get; private set; } = string.Empty; // MaxLength=999, Default="Descriptive Description", Rule=NotEmpty().MinimumLength(2).MaximumLength(1000)
    public Guid? RationId { get; private set; } = Guid.Empty; // Rule=NotEmpty()     public virtual Ration Ration { get; private set; } = default!;
    public Guid? GrowthTreatmentId { get; private set; } = Guid.Empty; // Rule=NotEmpty()     public virtual GrowthTreatment GrowthTreatment { get; private set; } = default!;
    public Guid? PreventiveTreatmentId { get; private set; } = Guid.Empty; // Rule=NotEmpty()     public virtual PreventiveTreatment PreventiveTreatment { get; private set; } = default!;

    private LifecycleStage() { }

    private LifecycleStage(
        Guid id,
        string name,
        string? description,
        Guid? rationId,
        Guid? growthTreatmentId,
        Guid? preventiveTreatmentId
    )
    {
        Id = id;
        Name = name;
        Description = description;
        RationId = rationId;
        GrowthTreatmentId = growthTreatmentId;
        PreventiveTreatmentId = preventiveTreatmentId;

        QueueDomainEvent(new LifecycleStageCreated {LifecycleStage = this});
    }

    public static LifecycleStage Create(
        string name,
        string? description,
        Guid? rationId,
        Guid? growthTreatmentId,
        Guid? preventiveTreatmentId
    )
    {
        return new LifecycleStage(
            Guid.NewGuid(),
            name,
            description,
            rationId,
            growthTreatmentId,
            preventiveTreatmentId
        );
    }

    public LifecycleStage Update(
        string name,
        string? description,
        Guid? rationId,
        Guid? growthTreatmentId,
        Guid? preventiveTreatmentId
    )
    {
        Name = name;
        Description = description;
        RationId = rationId;
        GrowthTreatmentId = growthTreatmentId;
        PreventiveTreatmentId = preventiveTreatmentId;
        QueueDomainEvent(new LifecycleStageUpdated { LifecycleStage = this });
        return this;
    }
}


