using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Members.Domain.MemberAds.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Members.Domain.MemberAds;
public class MemberAd : AuditableEntity, IAggregateRoot, ITenantEntity, IPublicEntity
{
    public string? TenantId { get; set; } = string.Empty; // Rule=NotEmpty()
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)


    private MemberAd() { }

    private MemberAd(
        Guid id,
        string? tenantId,
        string name,
        string? description
    )
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Description = description;

        QueueDomainEvent(new MemberAdCreated {MemberAd = this});
    }

    public static MemberAd Create(
        string? tenantId,
        string name,
        string? description
    )
    {
        var newId = Guid.NewGuid();
        
        return new MemberAd(
            newId,
            tenantId,
            name,
            description
        );
    }

    public MemberAd Update(
        string? tenantId,
        string name,
        string? description
    )
    {
        TenantId = tenantId;
        Name = name;
        Description = description;
        QueueDomainEvent(new MemberAdUpdated { MemberAd = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


