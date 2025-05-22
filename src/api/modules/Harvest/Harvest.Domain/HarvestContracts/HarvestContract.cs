using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
public class HarvestContract : AuditableEntity, IAggregateRoot, ITenantEntity, IShareableEntity
{
    public string? TenantId { get; set; } = string.Empty;
    public Guid? MemberId { get; set; } = Guid.Empty;
    [SharedWithMembers]
    public List<HarvestContractHarvestMember> HarvestContractHarvestMembers { get; set; } = new();
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)
    public Guid? HarvestContractStatusId { get; private set; } = Guid.Empty; // Rule=NotEmpty()
    public HarvestContractStatus HarvestContractStatus { get; private set; } = default!;

    private HarvestContract() { }

    private HarvestContract(
        Guid id,
        string? tenantId,
        Guid? memberId,
        List<HarvestContractHarvestMember> harvestContractHarvestMembers,
        string name,
        string? description,
        Guid? harvestContractStatusId
    )
    {
        Id = id;
        TenantId = tenantId;
        MemberId = memberId;
        HarvestContractHarvestMembers = harvestContractHarvestMembers;
        Name = name;
        Description = description;
        HarvestContractStatusId = harvestContractStatusId;

        QueueDomainEvent(new HarvestContractCreated {HarvestContract = this});
    }

    public static HarvestContract Create(
        string? tenantId,
        Guid? memberId,
        List<HarvestContractHarvestMember> harvestContractHarvestMembers,
        string name,
        string? description,
        Guid? harvestContractStatusId
    )
    {
        var newId = Guid.NewGuid();
        foreach(var i in harvestContractHarvestMembers)
        {
            i.HarvestContractId = newId;
        }
        return new HarvestContract(
            newId,
            tenantId,
            memberId,
            harvestContractHarvestMembers,
            name,
            description,
            harvestContractStatusId
        );
    }

    public HarvestContract Update(
        string? tenantId,
        Guid? memberId,
        List<HarvestContractHarvestMember> harvestContractHarvestMembers,
        string name,
        string? description,
        Guid? harvestContractStatusId
    )
    {
        TenantId = tenantId;
        MemberId = memberId;
        HarvestContractHarvestMembers = harvestContractHarvestMembers;
        Name = name;
        Description = description;
        HarvestContractStatusId = harvestContractStatusId;
        QueueDomainEvent(new HarvestContractUpdated { HarvestContract = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


