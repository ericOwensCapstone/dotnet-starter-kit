using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;

namespace FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Events;
public sealed record ContractStatusUpdated : DomainEvent
{
    public ContractStatus? ContractStatus { get; set; }
}

