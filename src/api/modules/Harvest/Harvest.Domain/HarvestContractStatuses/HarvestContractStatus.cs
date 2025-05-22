using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
public class HarvestContractStatus : AuditableEntity, IAggregateRoot, ITenantEntity, IPublicEntity
{
    public string? TenantId { get; set; } = string.Empty;
    public Guid? MemberId { get; set; } = Guid.Empty;
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)


    private HarvestContractStatus() { }

    private HarvestContractStatus(
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

        QueueDomainEvent(new HarvestContractStatusCreated {HarvestContractStatus = this});
    }

    public static HarvestContractStatus Create(
        string? tenantId,
        Guid? memberId,
        string name,
        string? description
    )
    {
        var newId = Guid.NewGuid();
        
        return new HarvestContractStatus(
            newId,
            tenantId,
            memberId,
            name,
            description
        );
    }

    public HarvestContractStatus Update(
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
        QueueDomainEvent(new HarvestContractStatusUpdated { HarvestContractStatus = this });
        return this;
    }
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = !context.Set<HarvestContract>().Any(ls => ls.HarvestContractStatusId == Id && ls.Deleted == null);
        string reason = canBeDeleted ? string.Empty : "Cannot soft delete HarvestContractStatus because it is referenced by a non-soft-deleted HarvestContract.";
        return (canBeDeleted, reason);
    }
}


