using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.RationCatalog.Domain.Events;

namespace FSH.Starter.WebApi.RationCatalog.Domain;
public class Ration : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal DollarsPerPound { get; private set; }

    public static Ration Create(string name, string? description, decimal dollarsPerPound)
    {
        var ration = new Ration();

        ration.Name = name;
        ration.Description = description;
        ration.DollarsPerPound = dollarsPerPound;

        ration.QueueDomainEvent(new RationCreated() { Ration = ration });

        return ration;
    }

    public static Ration Create(Guid id, string name, string? description, decimal dollarsPerPound)
    {
        var ration = new Ration
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerPound = dollarsPerPound
        };

        return ration;
    }

    public Ration Update(string? name, string? description, decimal? dollarsPerPound)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (dollarsPerPound.HasValue && DollarsPerPound != dollarsPerPound) DollarsPerPound = dollarsPerPound.Value;

        this.QueueDomainEvent(new RationUpdated() { Ration = this });
        return this;
    }

    public static Ration Update(Guid id, string name, string? description, decimal dollarsPerPound)
    {
        var ration = new Ration
        {
            Id = id,
            Name = name,
            Description = description,
            DollarsPerPound = dollarsPerPound
        };

        ration.QueueDomainEvent(new RationUpdated() { Ration = ration });

        return ration;
    }
}

//using System;

//public class Ration : IEquatable<Ration>
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public string Description { get; set; }

//    public override bool Equals(object obj)
//    {
//        if (obj is Ration other)
//        {
//            return Equals(other);
//        }
//        return false;
//    }

//    public bool Equals(Ration other)
//    {
//        if (other == null)
//        {
//            return false;
//        }

//        return Id == other.Id &&
//               Name == other.Name &&
//               Description == other.Description;
//    }

//    public override int GetHashCode()
//    {
//        return HashCode.Combine(Id, Name, Description);
//    }

//    public static bool operator ==(Ration left, Ration right)
//    {
//        if (ReferenceEquals(left, right))
//        {
//            return true;
//        }

//        if (left is null || right is null)
//        {
//            return false;
//        }

//        return left.Equals(right);
//    }

//    public static bool operator !=(Ration left, Ration right)
//    {
//        return !(left == right);
//    }
//}


