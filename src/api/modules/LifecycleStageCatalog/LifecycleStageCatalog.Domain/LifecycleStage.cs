using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Events;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
public class LifecycleStage : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public Ration Ration { get; private set; }
    public GrowthTreatment GrowthTreatment { get; private set; }
    public PreventativeTreatment PreventativeTreatment { get; private set; }

    public decimal Rating { get; private set; }

    public static LifecycleStage Create
    (
        string name, 
        string? description, 
        decimal rating,
        Ration ration,
        GrowthTreatment growthTreatment,
        PreventativeTreatment preventativeTreatment
    )
    {
        var lifecycleStage = new LifecycleStage();

        lifecycleStage.Name = name;
        lifecycleStage.Description = description;
        lifecycleStage.Ration = ration;
        lifecycleStage.GrowthTreatment = growthTreatment;
        lifecycleStage.PreventativeTreatment = preventativeTreatment;

        lifecycleStage.Rating = rating;

        lifecycleStage.QueueDomainEvent(new LifecycleStageCreated() { LifecycleStage = lifecycleStage });

        return lifecycleStage;
    }

    public LifecycleStage Update
    (
        string name,
        string? description,
        decimal? rating,
        Ration ration,
        GrowthTreatment growthTreatment,
        PreventativeTreatment preventativeTreatment
    )
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (rating.HasValue && Rating != rating) Rating = rating.Value;
        if (ration is not null && Ration.Id != ration.Id) Ration = ration;
        if (growthTreatment is not null && GrowthTreatment.Id != growthTreatment.Id) GrowthTreatment = growthTreatment;
        if (preventativeTreatment is not null && PreventativeTreatment.Id != preventativeTreatment.Id) PreventativeTreatment = preventativeTreatment;
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

