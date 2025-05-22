using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Events;
public sealed record HarvestContractStatusCreated : DomainEvent
{
    public HarvestContractStatus? HarvestContractStatus { get; set; }
}

