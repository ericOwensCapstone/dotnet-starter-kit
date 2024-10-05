using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Events;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
public class GrowthTreatment : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal DollarsPerHead { get; private set; }

    public static GrowthTreatment Create(string name, string? description, decimal dollarsperhead)
    {
        var growthTreatment = new GrowthTreatment();

        growthTreatment.Name = name;
        growthTreatment.Description = description;
        growthTreatment.DollarsPerHead = dollarsperhead;

        growthTreatment.QueueDomainEvent(new GrowthTreatmentCreated() { GrowthTreatment = growthTreatment });

        return growthTreatment;
    }

    public static GrowthTreatment CreateWithGuid(Guid id, string name, string? description, decimal dollarsperhead)
    {
        var growthTreatment = new GrowthTreatment
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerHead = dollarsperhead
        };

        return growthTreatment;
    }

    public GrowthTreatment Update(string? name, string? description, decimal? dollarsperhead)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (dollarsperhead.HasValue && DollarsPerHead != dollarsperhead) DollarsPerHead = dollarsperhead.Value;

        this.QueueDomainEvent(new GrowthTreatmentUpdated() { GrowthTreatment = this });
        return this;
    }

    public static GrowthTreatment Update(Guid id, string name, string? description, decimal dollarsperhead)
    {
        var growthTreatment = new GrowthTreatment
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerHead = dollarsperhead
        };

        growthTreatment.QueueDomainEvent(new GrowthTreatmentUpdated() { GrowthTreatment = growthTreatment });

        return growthTreatment;
    }
}

