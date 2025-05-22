using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Events;
public sealed record HarvestContractCreated : DomainEvent
{
    public HarvestContract? HarvestContract { get; set; }
}

