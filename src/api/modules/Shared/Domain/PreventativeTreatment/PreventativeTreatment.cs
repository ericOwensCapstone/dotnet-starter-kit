using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Events;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
public class PreventativeTreatment : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal DollarsPerHead { get; private set; }

    public static PreventativeTreatment Create(string name, string? description, decimal dollarsperhead)
    {
        var preventativeTreatment = new PreventativeTreatment();

        preventativeTreatment.Name = name;
        preventativeTreatment.Description = description;
        preventativeTreatment.DollarsPerHead = dollarsperhead;

        preventativeTreatment.QueueDomainEvent(new PreventativeTreatmentCreated() { PreventativeTreatment = preventativeTreatment });

        return preventativeTreatment;
    }

    public static PreventativeTreatment Create(Guid id, string name, string? description, decimal dollarsperhead)
    {
        var preventativeTreatment = new PreventativeTreatment
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerHead = dollarsperhead
        };

        return preventativeTreatment;
    }

    public PreventativeTreatment Update(string? name, string? description, decimal? dollarsperhead)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (dollarsperhead.HasValue && DollarsPerHead != dollarsperhead) DollarsPerHead = dollarsperhead.Value;

        this.QueueDomainEvent(new PreventativeTreatmentUpdated() { PreventativeTreatment = this });
        return this;
    }

    public static PreventativeTreatment Update(Guid id, string name, string? description, decimal dollarsperhead)
    {
        var preventativeTreatment = new PreventativeTreatment
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerHead = dollarsperhead
        };

        preventativeTreatment.QueueDomainEvent(new PreventativeTreatmentUpdated() { PreventativeTreatment = preventativeTreatment });

        return preventativeTreatment;
    }
}

