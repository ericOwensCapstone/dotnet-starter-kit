using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
public class ContractStatus : AuditableEntity, IAggregateRoot, ITenantEntity, IPublicEntity
{
    public string? TenantId { get; set; } = string.Empty;
    public Guid? MemberId { get; set; } = Guid.Empty;
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)


    private ContractStatus() { }

    private ContractStatus(
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

        QueueDomainEvent(new ContractStatusCreated {ContractStatus = this});
    }

    public static ContractStatus Create(
        string? tenantId,
        string name,
        string? description
    )
    {
        var newId = Guid.NewGuid();
        
        return new ContractStatus(
            newId,
            tenantId,
            name,
            description
        );
    }

    public ContractStatus Update(
        string? tenantId,
        string name,
        string? description
    )
    {
        TenantId = tenantId;
        Name = name;
        Description = description;
        QueueDomainEvent(new ContractStatusUpdated { ContractStatus = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


