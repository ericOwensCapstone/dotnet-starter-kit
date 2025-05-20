using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using FSH.Framework.Core.Tenant.Abstractions;

namespace FSH.Starter.WebApi.Ranch.Domain.Contracts;

public class ContractMemberPage
{
    public Guid ContractId { get; set; }
    public Guid MemberPageId { get; set; }
    [PropertyWithMemberId]
    public MemberPage MemberPage { get; set; } = default!;
}

