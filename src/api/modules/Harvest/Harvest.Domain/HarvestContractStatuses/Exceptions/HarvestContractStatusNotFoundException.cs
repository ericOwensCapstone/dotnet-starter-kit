using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Exceptions;
public sealed class HarvestContractStatusNotFoundException : NotFoundException
{
    public HarvestContractStatusNotFoundException(Guid id)
        : base($"harvestContractStatus with id {id} not found")
    {
    }
}

