using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Ranch.Domain.Contracts.Events;
using Microsoft.EntityFrameworkCore;

namespace FSH.Starter.WebApi.Ranch.Domain.Contracts;
public class Contract : AuditableEntity, IAggregateRoot, ITenantEntity, IShareableEntity
{
    public string? TenantId { get; set; } = string.Empty;
    public Guid? MemberId { get; set; } = Guid.Empty;
    //TODO Shared
    [SharedWithMembers]
    public List<ContractMemberPage> ContractMemberPages { get; set; } = new();
    public string Name { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(99)
    public string? Description { get; private set; } = string.Empty; // Rule=NotEmpty().MinimumLength(2).MaximumLength(999)
    public Guid? ContractStatusId { get; private set; } = Guid.Empty; // Rule=NotEmpty()
    public virtual ContractStatus ContractStatus { get; private set; } = default!;

    private Contract() { }

    private Contract(
        Guid id,
        string? tenantId,
        Guid? memberId,
        List<ContractMemberPage> contractMemberPages,
        string name,
        string? description,
        Guid? contractStatusId
    )
    {
        Id = id;
        TenantId = tenantId;
        MemberId = memberId;
        ContractMemberPages = contractMemberPages;
        Name = name;
        Description = description;
        ContractStatusId = contractStatusId;

        QueueDomainEvent(new ContractCreated {Contract = this});
    }

    public static Contract Create(
        string? tenantId,
        Guid? memberId,
        List<ContractMemberPage> contractMemberPages,
        string name,
        string? description,
        Guid? contractStatusId
    )
    {
        var newId = Guid.NewGuid();
        foreach(var i in contractMemberPages)
        {
            i.ContractId = newId;
        }
        return new Contract(
            newId,
            tenantId,
            memberId,
            contractMemberPages,
            name,
            description,
            contractStatusId
        );
    }

    public Contract Update(
        string? tenantId,
        Guid? memberId,
        List<ContractMemberPage> contractMemberPages,
        string name,
        string? description,
        Guid? contractStatusId
    )
    {
        TenantId = tenantId;
        MemberId = memberId;
        ContractMemberPages = contractMemberPages;
        Name = name;
        Description = description;
        ContractStatusId = contractStatusId;
        QueueDomainEvent(new ContractUpdated { Contract = this });
        return this;
    }
    
    public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        bool canBeDeleted = true;
        string reason = string.Empty;
        return (canBeDeleted, reason);
    }
}


