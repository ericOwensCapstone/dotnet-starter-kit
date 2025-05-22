using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Exceptions;
public sealed class HarvestContractNotFoundException : NotFoundException
{
    public HarvestContractNotFoundException(Guid id)
        : base($"harvestContract with id {id} not found")
    {
    }
}

