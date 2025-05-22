using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Events;
public sealed record HarvestMemberUpdated : DomainEvent
{
    public HarvestMember? HarvestMember { get; set; }
}

