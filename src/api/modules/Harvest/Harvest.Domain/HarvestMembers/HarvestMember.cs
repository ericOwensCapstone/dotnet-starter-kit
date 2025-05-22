using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
public class HarvestMember : AuditableEntity, IAggregateRoot, ITenantEntity, IPublicEntity
{
    public string? TenantId { get; set; } = string.Empty; //
    public Guid? MemberId { get; set; } = Guid.Empty; //
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)


    private HarvestMember() { }

    private HarvestMember(
        Guid id,
        string? tenantId,
        Guid? memberId,
        string name,
        string? description
    )
    {
        Id = id;
        TenantId = tenantId;
        MemberId = memberId;
        Name = name;
        Description = description;

        QueueDomainEvent(new HarvestMemberCreated {HarvestMember = this});
    }

    public static HarvestMember Create(
        string? tenantId,
        Guid? memberId,
        string name,
        string? description
    )
    {
        var newId = Guid.NewGuid();
        
        return new HarvestMember(
            newId,
            tenantId,
            memberId,
            name,
            description
        );
    }

    public HarvestMember Update(
        string? tenantId,
        Guid? memberId,
        string name,
        string? description
    )
    {
        TenantId = tenantId;
        MemberId = memberId;
        Name = name;
        Description = description;
        QueueDomainEvent(new HarvestMemberUpdated { HarvestMember = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


