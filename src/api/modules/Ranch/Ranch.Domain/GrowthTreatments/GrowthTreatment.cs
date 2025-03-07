using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments.Events;

namespace FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
public class GrowthTreatment : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }

    private GrowthTreatment() { }

    private GrowthTreatment(Guid id, string name, string? description, decimal price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;

        QueueDomainEvent(new GrowthTreatmentCreated { GrowthTreatment = this });
    }

    public static GrowthTreatment Create(string name, string? description, decimal price)
    {
        return new GrowthTreatment(Guid.NewGuid(), name, description, price);
    }

    public GrowthTreatment Update(string? name, string? description, decimal? price)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
        {
            Name = name;
            isUpdated = true;
        }

        if (!string.Equals(Description, description, StringComparison.OrdinalIgnoreCase))
        {
            Description = description;
            isUpdated = true;
        }

        if (price.HasValue && Price != price.Value)
        {
            Price = price.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new GrowthTreatmentUpdated { GrowthTreatment = this });
        }

        return this;
    }
}


