using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Members.Domain.MemberAds;

namespace FSH.Starter.WebApi.Members.Domain.MemberAds.Events;
public sealed record MemberAdUpdated : DomainEvent
{
    public MemberAd? MemberAd { get; set; }
}

