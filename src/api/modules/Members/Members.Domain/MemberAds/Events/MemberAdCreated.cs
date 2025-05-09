using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Members.Domain.MemberAds;

namespace FSH.Starter.WebApi.Members.Domain.MemberAds.Events;
public sealed record MemberAdCreated : DomainEvent
{
    public MemberAd? MemberAd { get; set; }
}

