using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;

namespace FSH.Starter.WebApi.Ranch.Domain.MemberPages.Events;
public sealed record MemberPageUpdated : DomainEvent
{
    public MemberPage? MemberPage { get; set; }
}

