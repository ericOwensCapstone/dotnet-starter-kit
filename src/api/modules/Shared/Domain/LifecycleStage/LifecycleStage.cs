using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
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

    public double TargetWeight { get; private set; }
    public double TargetAdfi { get; private set; }
    public double AdfiStdDev { get; private set; }
    public double TargetWeightRangeForSort { get; private set; }
    public int MergeableDuration { get; private set; }
    public double MergeableWeightRange { get; private set; }
    public int MaxHead { get; private set; }

    public static LifecycleStage Create
    (
        string name, 
        string? description, 
        Ration ration,
        GrowthTreatment growthTreatment,
        PreventativeTreatment preventativeTreatment,
        double targetWeight,
        double targetAdfi,
        double adfiStdDev,
        double targetWeightRangeForSort,
        int mergeableDuration,
        double mergeableWeightRange,
        int maxHead
    )
    {
        var lifecycleStage = new LifecycleStage();

        lifecycleStage.Name = name;
        lifecycleStage.Description = description;
        lifecycleStage.Ration = ration;
        lifecycleStage.GrowthTreatment = growthTreatment;
        lifecycleStage.PreventativeTreatment = preventativeTreatment;

        lifecycleStage.TargetWeight = targetWeight;
        lifecycleStage.TargetAdfi = targetAdfi;
        lifecycleStage.AdfiStdDev = adfiStdDev;
        lifecycleStage.MergeableDuration = mergeableDuration;
        lifecycleStage.MergeableWeightRange = mergeableWeightRange;
        lifecycleStage.MaxHead = maxHead;

        lifecycleStage.QueueDomainEvent(new LifecycleStageCreated() { LifecycleStage = lifecycleStage });

        return lifecycleStage;
    }

    public LifecycleStage Update
    (
        string name,
        string? description,
        Ration ration,
        GrowthTreatment growthTreatment,
        PreventativeTreatment preventativeTreatment,
        double targetWeight,
        double targetAdfi,
        double adfiStdDev,
        double targetWeightRangeForSort,
        int mergeableDuration,
        double mergeableWeightRange,
        int maxHead
    )
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (ration is not null && Ration.Id != ration.Id) Ration = ration;
        if (growthTreatment is not null && GrowthTreatment.Id != growthTreatment.Id) GrowthTreatment = growthTreatment;
        if (preventativeTreatment is not null && PreventativeTreatment.Id != preventativeTreatment.Id) PreventativeTreatment = preventativeTreatment;
        if (targetWeight != TargetWeight) TargetWeight = targetWeight;
        if (targetAdfi != TargetAdfi) TargetAdfi = targetAdfi;
        if (adfiStdDev != AdfiStdDev) AdfiStdDev = adfiStdDev;
        if (targetWeightRangeForSort != TargetWeightRangeForSort) TargetWeightRangeForSort = targetWeightRangeForSort;
        if (mergeableDuration != MergeableDuration) MergeableDuration = mergeableDuration;
        if (mergeableWeightRange != MergeableWeightRange) MergeableWeightRange = mergeableWeightRange;
        if (maxHead != MaxHead) MaxHead = maxHead;
        this.QueueDomainEvent(new LifecycleStageUpdated() { LifecycleStage = this });
        return this;
    }

    public static LifecycleStage Update
    (
        Guid id, 
        string name, 
        string? description,
        Ration ration,
        GrowthTreatment growthTreatment,
        PreventativeTreatment preventativeTreatment,
        double targetWeight,
        double targetAdfi,
        double adfiStdDev,
        double targetWeightRangeForSort,
        int mergeableDuration,
        double mergeableWeightRange,
        int maxHead
    )
    {
        var lifecycleStage = new LifecycleStage
        {
            Id = id,
            Name = name,
            Description = description,
            Ration = ration,
            GrowthTreatment = growthTreatment,
            PreventativeTreatment = preventativeTreatment,
            TargetWeight = targetWeight,
            TargetAdfi = targetAdfi,
            AdfiStdDev = adfiStdDev,
            MergeableDuration = mergeableDuration,
            MergeableWeightRange = mergeableWeightRange,
            MaxHead = maxHead
        };

        lifecycleStage.QueueDomainEvent(new LifecycleStageUpdated() { LifecycleStage = lifecycleStage });

        return lifecycleStage;
    }
}

