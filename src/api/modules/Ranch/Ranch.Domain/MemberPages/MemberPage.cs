using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Ranch.Domain.MemberPages;
public class MemberPage : AuditableEntity, IAggregateRoot, ITenantEntity, IPublicEntity
{
    public string? TenantId { get; set; } = string.Empty; //
    public Guid? MemberId { get; set; } = Guid.Empty; //
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)


    private MemberPage() { }

    private MemberPage(
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

        QueueDomainEvent(new MemberPageCreated {MemberPage = this});
    }

    public static MemberPage Create(
        string? tenantId,
        Guid? memberId,
        string name,
        string? description
    )
    {
        var newId = Guid.NewGuid();
        
        return new MemberPage(
            newId,
            tenantId,
            memberId,
            name,
            description
        );
    }

    public MemberPage Update(
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
        QueueDomainEvent(new MemberPageUpdated { MemberPage = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        // Tenant validation is handled in the endpoint and via permissions
        // This is just a safety check for any entities that should never be deleted
        
        // Additional validation logic could be added here if needed,
        // such as checking if MemberPage is referenced by important entities
        
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


